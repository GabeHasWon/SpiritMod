using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Clubs
{
	public abstract class ClubProj : ModProjectile
	{
		public readonly int ChargeTime;
		private readonly int DustType;
		private readonly int Size;
		private readonly float Acceleration;
		private readonly float MaxSpeed;

		public float minKnockback;
		public float maxKnockback;

		public int minDamage;
		public int maxDamage;

		public ClubProj(int chargetime, int dusttype, int size, float acceleration, float maxspeed = -1)
		{
			ChargeTime = chargetime;
			DustType = dusttype;
			Size = size;
			Acceleration = acceleration;
			MaxSpeed = maxspeed;
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
			Projectile.alpha = 255;
			SafeSetDefaults();
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].Center, Projectile.Center) ? true : base.Colliding(projHitbox, targetHitbox);
		public virtual void Smash(Vector2 position) { }

		public bool released = false;
		public double radians = 0;

		public float animTime;

		private float _angularMomentum = 1;
		private int _lingerTimer = 0;
		private int _flickerTime = 0;

		public SpriteEffects Effects => ((Main.player[Projectile.owner].direction * (int)Main.player[Projectile.owner].gravDir) < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		public float TrueRotation => (float)radians + 3.9f + ((Effects == SpriteEffects.FlipHorizontally) ? MathHelper.PiOver2 : 0);
		public Vector2 Origin => (Effects == SpriteEffects.FlipHorizontally) ? new Vector2(Size, Size) : new Vector2(0, Size);

		public sealed override bool PreDraw(ref Color lightColor)
		{
			if (released && _lingerTimer == 0)
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
					Color trailColor = lightColor * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * .5f;
					Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, new Rectangle(0, 0, Size, Size), trailColor, Projectile.oldRot[k], Origin, Projectile.scale, Effects, 0);
				}
			}

			Color color = lightColor;
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, 0, Size, Size), color, TrueRotation, Origin, Projectile.scale, Effects, 0);
			
			SafeDraw(Main.spriteBatch, lightColor);
			
			if (Projectile.ai[0] >= ChargeTime && !released && _flickerTime < 16)
			{
				_flickerTime++;
				color = Color.White;
				float flickerTime2 = _flickerTime / 20f;
				float alpha = 1.5f - ((flickerTime2 * flickerTime2 / 2) + (2f * flickerTime2));
				if (alpha < 0)
					alpha = 0;

				Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, Size, Size, Size), color * alpha, TrueRotation, Origin, Projectile.scale, Effects, 1);
			}
			return false;
		}

		public sealed override bool PreAI()
		{
			SafeAI();

			Player player = Main.player[Projectile.owner];
			int animMax = 120;
			int animMaxHalf = animMax / 2;

			Projectile.scale = (Projectile.ai[0] < 10 && !released) ? (Projectile.ai[0] / 10f) : 1;
			player.heldProj = Projectile.whoAmI;

			if (player.HeldItem.useTurn)
			{
				player.direction = Math.Sign(player.velocity.X);
				if (player.direction == 0)
					player.direction = player.oldDirection;

				Projectile.velocity.X = player.direction;
			}

			float degrees = (float)((animTime * -1.12) + 84) * player.direction * (int)player.gravDir;
			if (player.direction == 1)
				degrees += 180;

			radians = degrees * (Math.PI / 180);
			if (player.channel && !released)
			{
				if (Projectile.ai[0] == 0)
				{
					animTime = animMax;
				}
				if (Projectile.ai[0] < ChargeTime)
				{
					Projectile.ai[0]++;
					float rot = Main.rand.NextFloat(MathHelper.TwoPi);
					
					if (DustType != -1)
						Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * 35, DustType, -Vector2.One.RotatedBy(rot) * 1.5f, 0, default, Projectile.ai[0] / 100f);

					_angularMomentum = -1;
				}
				else
				{
					if (Projectile.ai[0] == ChargeTime)
					{
						for (int k = 0; k <= 100; k++)
						{
							if (DustType != -1)
								Dust.NewDustPerfect(Projectile.Center, DustType, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2), 0, default, 1.5f);
						}
						SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);
						Projectile.ai[0]++;
					}
					if (DustType != -1)
						Dust.NewDustPerfect(Projectile.Center, DustType, Vector2.One.RotatedByRandom(MathHelper.TwoPi));

					_angularMomentum = MathHelper.Lerp(_angularMomentum, 0, 0.08f);
				}

				float dmg = minDamage + (Projectile.ai[0] / ChargeTime * (maxDamage - minDamage));
				Projectile.damage = (int)player.GetDamage(DamageClass.Melee).ApplyTo(dmg);
				Projectile.knockBack = minKnockback + (int)(Projectile.ai[0] / ChargeTime * (maxKnockback - minKnockback));
			}
			else
			{
				float acceleration = Acceleration * (float)(MathHelper.Clamp(1f - (float)(animTime / animMaxHalf), 0f, 1f) + 0.1f);

				if (_angularMomentum < MaxSpeed || MaxSpeed < 0)
					_angularMomentum += acceleration;

				if (!released)
				{
					released = true;

					Projectile.friendly = true;
					SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
				}
			}

			Projectile.position.Y = player.Center.Y - (int)(Math.Sin(radians * 0.96) * Size) - (Projectile.height / 2);
			Projectile.position.X = player.Center.X - (int)(Math.Cos(radians * 0.96) * Size) - (Projectile.width / 2);

			float rotation = (float)radians + 1.7f;
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);
			player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);

			if (_lingerTimer == 0)
			{
				if (animTime > _angularMomentum + 1)
					animTime -= _angularMomentum;
				else
					animTime = 2;

				Tile tile = Main.tile[(int)Projectile.Center.X / 16, (int)((Projectile.Center.Y + (Projectile.height * 0.5f)) / 16)];
				bool validTile = tile.HasTile && tile.BlockType == BlockType.Solid && Main.tileSolid[tile.TileType];
				if (animTime == 2 || (validTile && released && animTime <= animMaxHalf))
				{
					_lingerTimer = 30;

					if (Projectile.ai[0] >= ChargeTime)
						Smash(Projectile.Center);

					if (validTile)
						player.GetModPlayer<MyPlayer>().Shake += (int)(Projectile.ai[0] * 0.2f);

					Projectile.friendly = false;
					SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
					SoundEngine.PlaySound(SoundID.NPCHit42, Projectile.Center);
				}
			}
			else
			{
				_lingerTimer--;
				if (_lingerTimer == 1)
				{
					Projectile.active = false;
					animTime = 2;
				}

				float increment = _lingerTimer * 0.065f; //Allow collision overshoot

				animTime += (int)increment;
			}

			Projectile.rotation = TrueRotation; //This is set for drawing afterimages
			player.itemAnimation = player.itemTime = (int)animTime;

			return true;
		}
	}
}