using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using SpiritMod.Mechanics.Trails;

namespace SpiritMod.Projectiles.Clubs.BruteHammer
{
	public class BruteHammerProj : ModProjectile, IManualTrailProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brute Hammer");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

		}
		public override void SetDefaults()
		{
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.ownerHitCheck = true;
		}

		public void DoTrailCreation(TrailManager tM)
			=> tM.CreateCustomTrail(new BruteHammerTrail(Projectile, new Color(123, 19, 19), 18, 10));

		private readonly Point Size = new(50, 80);

		private float spinTimer;

		public int minDamage;
		public int maxDamage;

		public int minKnockback = 4;
		public int maxKnockback = 8;

		private const float maxSpeed = 15f;
		private const float acceleration = 2f;

		private bool released = false;
		private double radians = 0;

		public float animTime;

		private float _angularMomentum = 1;
		private int _lingerTimer;
		private int _flickerTime = 0;

		private readonly int ChargeTime = 54;

		public SpriteEffects Effects => ((Main.player[Projectile.owner].direction * (int)Main.player[Projectile.owner].gravDir) < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		public float TrueRotation => (float)radians + ((Effects == SpriteEffects.FlipHorizontally) ? MathHelper.PiOver2 + 3.6f : 4.2f);
		public Vector2 Origin => (Effects == SpriteEffects.FlipHorizontally) ? new Vector2(Size.X, Size.Y) : new Vector2(0, Size.Y);

		public override bool PreDraw(ref Color lightColor)
		{
			if (_lingerTimer == 0)
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
					Color trailColor = lightColor * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * .5f;
					Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, new Rectangle(0, 0, Size.X, Size.Y), trailColor, Projectile.oldRot[k], Origin, Projectile.scale, Effects, 0);
				}
			}

			Color color = lightColor;
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, 0, Size.X, Size.Y), color, TrueRotation, Origin, Projectile.scale, Effects, 0);

			if (Projectile.ai[0] >= ChargeTime && !released && _flickerTime < 16)
			{
				_flickerTime++;
				color = Color.White;
				float flickerTime2 = _flickerTime / 20f;
				float alpha = 1.5f - ((flickerTime2 * flickerTime2 / 2) + (2f * flickerTime2));
				if (alpha < 0)
					alpha = 0;

				Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, Size.Y, Size.X, Size.Y), color * alpha, TrueRotation, Origin, Projectile.scale, Effects, 1);
			}
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.scale = (Projectile.ai[0] < 10 && !released) ? (Projectile.ai[0] / 10f) : 1;
			player.heldProj = Projectile.whoAmI;
			int animMax = 360;

			Projectile.timeLeft = 2;

			if (player.HeldItem.useTurn)
			{
				player.direction = Math.Sign(player.velocity.X);
				if (player.direction == 0)
					player.direction = player.oldDirection;

				Projectile.velocity.X = player.direction;
			}

			float degrees = (float)((animTime * -1.35) + 74) * player.direction * (int)player.gravDir;
			if (player.direction == 1)
				degrees += 180;

			radians = degrees * (Math.PI / 180);
			if (player.channel && !released)
			{
				if (Projectile.ai[0] == 0)
					animTime = animMax;

				if (Projectile.ai[0] <= ChargeTime)
					Projectile.ai[0]++;

				if (Projectile.ai[0] == ChargeTime)
					SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);

				_angularMomentum = 0;

				spinTimer += Projectile.ai[0] / (float)ChargeTime * 24f * player.direction;
				radians += spinTimer * (Math.PI / 180);
			}
			else
			{
				if (_angularMomentum < maxSpeed)
					_angularMomentum += acceleration;

				if (!released)
				{
					Projectile.damage = (int)(minDamage + (Projectile.ai[0] / ChargeTime * (maxDamage - minDamage)));
					Projectile.knockBack = minKnockback + (int)(Projectile.ai[0] / ChargeTime * (maxKnockback - minKnockback));

					TrailManager.ManualTrailSpawn(Projectile);

					released = true;
					Projectile.friendly = true;
					SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
				}
			}

			Projectile.position.X = player.Center.X - (int)(Math.Cos(radians * 0.96) * Size.X) - (Projectile.width / 2);
			Projectile.position.Y = player.Center.Y - (int)(Math.Sin(radians * 0.96) * Size.Y) - (Projectile.height / 2);

			float rotation = (float)radians + 1.7f;
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);
			player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);

			if (_lingerTimer == 0)
			{
				player.itemTime++;
				player.itemAnimation++;

				if (animTime > _angularMomentum + 1)
					animTime -= _angularMomentum;
				else
					animTime = 2;

				Tile tile = Main.tile[(int)Projectile.Center.X / 16, (int)((Projectile.Center.Y + (Projectile.width / 2)) / 16)];
				bool validTile = tile.HasTile && tile.BlockType == BlockType.Solid && Main.tileSolid[tile.TileType];
				if (animTime == 2 || (validTile && released && animTime <= (animMax / 3)))
				{
					_lingerTimer = 30;

					bool struckNPC = Projectile.numHits > 0;
					if (validTile || struckNPC)
					{
						player.GetModPlayer<MyPlayer>().Shake += (int)(Projectile.ai[0] * 0.2f);

						SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
						SoundEngine.PlaySound(SoundID.NPCHit42, Projectile.Center);

						if (Projectile.ai[0] >= ChargeTime)
						{
							for (int i = 0; i < 100; i++)
							{
								Vector2 position = Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2);
								Dust.NewDustPerfect(position, ModContent.DustType<Dusts.EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);

								if (i < 50)
									Dust.NewDustPerfect(position, DustID.Blood, player.DirectionTo(Projectile.Center).RotatedBy(Main.rand.NextFloat(-1.57f, 0f) * player.direction) * Main.rand.NextFloat(1.0f, 7.0f), Scale: Main.rand.NextFloat(0.5f, 1.5f));
							}
						}
					}
					else
					{
						SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/SwordSlash1") with { PitchVariance = 0.3f, Volume = 0.6f, Pitch = -0.7f }, Projectile.position);
					}

					Projectile.friendly = false;
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

				float increment = (int)(_lingerTimer * 0.065f); //Allow collision overshoot

				if (animTime <= 2) //The projectile has not collided with a tile
					increment = _lingerTimer * -0.15f;

				animTime += increment;
			}

			Projectile.rotation = TrueRotation; //This is set for drawing afterimages
			player.itemAnimation = player.itemTime = (int)animTime;
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (!released)
			{
				hitDirection = Math.Sign(target.Center.X - Main.player[Projectile.owner].Center.X);

				damage = (int)(damage * .6f);
				knockback *= .5f;
			}
		}
	}
}