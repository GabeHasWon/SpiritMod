using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Clubs
{
	public abstract class ClubProj : ModProjectile
	{
		public ClubProj(Vector2 size) { Size = size; }

		internal readonly Vector2 Size;

		public int ChargeTime { get; private set; }
		public float Acceleration { get; private set; }

		public int MinDamage { get; private set; }
		public int MaxDamage { get; private set; }

		public float MinKnockback { get; private set; }
		public float MaxKnockback { get; private set; }

		public void SetStats(int chargeTime, float acceleration, int minDamage, int maxDamage, float minKnockback, float maxKnockback)
		{
			ChargeTime = chargeTime;
			Acceleration = acceleration;
			MinDamage = minDamage;
			MaxDamage = maxDamage;
			MinKnockback = minKnockback;
			MaxKnockback = maxKnockback;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(animTime);
			writer.Write(_angularMomentum);
			writer.Write(_lingerTimer);
			writer.Write(released);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			animTime = reader.ReadSingle();
			_angularMomentum = reader.ReadSingle();
			_lingerTimer = reader.ReadInt32();
			released = reader.ReadBoolean();
		}

		public virtual void SafeAI() { }
		public virtual void SafeDraw(SpriteBatch spriteBatch, Color lightColor) { }
		public virtual void SafeSetDefaults() { }
		public virtual void SafeSetStaticDefaults() { }

		public sealed override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 2;

			SafeSetStaticDefaults();
		}

		public sealed override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.width = Projectile.height = 48;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;

			SafeSetDefaults();
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Player player = Main.player[Projectile.owner];
			Rectangle playerHitbox = player.getRect();

			if (Collision.CanHit(playerHitbox.TopLeft(), playerHitbox.Width, playerHitbox.Height, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height))
				return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, Projectile.Center) ? true : base.Colliding(projHitbox, targetHitbox);
			else
				return false;
		}

		public virtual void Smash(Vector2 position) { }

		public bool released = false;
		public double radians = 0;

		public float animTime;
		public int animMax = 120;

		private float _angularMomentum = 1;
		protected int _lingerTimer = 0;
		protected int _flickerTime = 0;

		public virtual SpriteEffects Effects => ((Main.player[Projectile.owner].direction * (int)Main.player[Projectile.owner].gravDir) < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		public virtual Vector2 Origin => (Effects == SpriteEffects.FlipHorizontally) ? Size : new Vector2(0, Size.Y);
		public virtual float TrueRotation => (float)radians + 3.9f + ((Effects == SpriteEffects.FlipHorizontally) ? MathHelper.PiOver2 : 0);

		public sealed override bool PreDraw(ref Color lightColor)
		{
			DrawTrail(lightColor);

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, texture.Frame(1, Main.projFrames[Type]), Projectile.GetAlpha(lightColor), Projectile.rotation, Origin, Projectile.scale, Effects, 0);
			
			SafeDraw(Main.spriteBatch, lightColor);
			
			if (Projectile.ai[0] >= ChargeTime && !released && _flickerTime < 16)
			{
				float flickerTime2 = ++_flickerTime / 20f;
				float alpha = Math.Max(0, 1.5f - ((flickerTime2 * flickerTime2 / 2) + (2f * flickerTime2)));

				Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, texture.Frame(1, Main.projFrames[Type], 0, 1, 0, 0), Projectile.GetAlpha(Color.White * alpha), Projectile.rotation, Origin, Projectile.scale, Effects, 1);
			}
			return false;
		}

		public virtual void DrawTrail(Color lightColor)
		{
			if (released && _lingerTimer == 0)
			{
				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
					Color trailColor = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * .5f;
					Main.EntitySpriteDraw(texture, drawPos, texture.Frame(1, Main.projFrames[Type]), trailColor, Projectile.oldRot[k], Origin, Projectile.scale, Effects, 0);
				}
			}
		}

		public sealed override bool PreAI()
		{
			SafeAI();

			Player owner = Main.player[Projectile.owner];

			if (owner.dead)
				Projectile.Kill();

			int animMaxHalf = animMax / 2;

			Projectile.scale = (Projectile.ai[0] < 10 && !released) ? (Projectile.ai[0] / 10f) : 1;
			owner.heldProj = Projectile.whoAmI;

			if (owner.HeldItem.useTurn)
			{
				if (!released)
				{
					if (owner == Main.LocalPlayer)
					{
						int newDir = Math.Sign(Main.MouseWorld.X - owner.Center.X);
						Projectile.velocity.X = (newDir == 0) ? owner.direction : newDir;

						if (newDir != owner.direction)
							Projectile.netUpdate = true;
					}

					owner.ChangeDir((int)Projectile.velocity.X);
				}
				else
					owner.direction = Math.Sign(Projectile.velocity.X);
			}

			float degrees = (float)((animTime * -1.12) + 84) * owner.direction * (int)owner.gravDir;
			if (owner.direction == 1)
				degrees += 180;

			float GetAcceleration() => Acceleration * (float)(MathHelper.Clamp(1f - (float)(animTime / animMaxHalf), 0f, 1f) + 0.1f);

			radians = degrees * (Math.PI / 180);
			if (owner.channel && !released)
			{
				if (Projectile.ai[0] < ChargeTime)
				{
					if (Projectile.ai[0] + 1 == ChargeTime)
						SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);
					if (Projectile.ai[0] == 0)
					{
						animTime = animMax;
						Projectile.netUpdate = true;
					}

					Projectile.ai[0]++;
					_angularMomentum = 50f / ChargeTime;
				}
				else
					_angularMomentum = MathHelper.Lerp(_angularMomentum, 0, 0.08f); //Slow to a stop after reaching full charge

				//Adjust stats gradually with charge
				Projectile.damage = (int)(MinDamage + (Projectile.ai[0] / ChargeTime * (MaxDamage - MinDamage)));
				Projectile.knockBack = MinKnockback + (int)(Projectile.ai[0] / ChargeTime * (MaxKnockback - MinKnockback));
			}
			else
			{
				if (Math.Abs(_angularMomentum) < 60)
					_angularMomentum -= GetAcceleration();

				if (!released)
				{
					released = true;

					Projectile.friendly = true;
					SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
				}
			}

			Projectile.position.X = owner.Center.X - (int)(Math.Cos(radians * 0.96) * Size.X) - (Projectile.width / 2);
			Projectile.position.Y = owner.Center.Y - (int)(Math.Sin(radians * 0.96) * Size.Y) - (Projectile.height / 2) - owner.gfxOffY;

			float rotation = (float)radians * owner.gravDir + 1.7f;
			owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);
			owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);

			if (_lingerTimer == 0)
			{
				bool swingEnded = animTime < 2;
				bool validTile = Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height, true);

				if (swingEnded || (validTile && released && animTime <= animMaxHalf))
				{
					_lingerTimer = 30;

					_angularMomentum = validTile ? -5 : 0;

					bool struckNPC = Projectile.numHits > 0;
					if (validTile || struckNPC)
					{
						owner.GetModPlayer<MyPlayer>().Shake += (int)(Projectile.ai[0] * 0.2f);

						SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
						SoundEngine.PlaySound(SoundID.NPCHit42, Projectile.Center);

						if (Projectile.ai[0] >= ChargeTime)
							Smash(Projectile.Center);
					}
					else if (Main.netMode != NetmodeID.Server)
					{
						SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/SwordSlash1") with { PitchVariance = 0.3f, Volume = 0.6f, Pitch = -0.7f }, Projectile.position);
					}

					Projectile.netUpdate = true;
					Projectile.friendly = false;
				}
			}
			else
			{
				_angularMomentum = (int)(_lingerTimer * 0.065f); //Allow overshoot on collision

				if (--_lingerTimer == 1)
				{
					Projectile.active = false;
					Projectile.netUpdate = true;

					animTime = 2;
				}
			}

			animTime += _angularMomentum * Math.Sign(animTime - (_angularMomentum + 1));

			Projectile.rotation = TrueRotation;
			owner.itemAnimation = owner.itemTime = 2;

			return true;
		}
	}
}