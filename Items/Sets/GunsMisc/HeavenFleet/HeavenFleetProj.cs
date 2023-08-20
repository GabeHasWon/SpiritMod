using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Particles;

namespace SpiritMod.Items.Sets.GunsMisc.HeavenFleet
{
	public class HeavenFleetProj : ModProjectile, IDrawAdditive
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			// DisplayName.SetDefault("Heaven Fleet");
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 999999;
			Projectile.ignoreWater = true;
			Projectile.alpha = 255;
		}

		int maxCounter = 1;
		bool firing = false;

		private static Vector2 GetOffset(float rotation, Player player)
		{
			var pos = player.Center - new Vector2(4, 4);
			return Vector2.Normalize(pos.DirectionTo(pos + new Vector2(1, 0).RotatedBy(rotation))) * 10f;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (player.whoAmI == Main.myPlayer)
			{
				Projectile.velocity = GetOffset(player.MountedCenter.AngleTo(Main.MouseWorld), player);

				Projectile.netUpdate = true;
			}

			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.spriteDirection = Projectile.direction;

			player.ChangeDir(Projectile.direction);

			player.itemTime = 10; // Set item time to 10 frames while we are used
			player.itemAnimation = 10; // Set item animation time to 10 frames while we are used

			Projectile.position = player.Center - Projectile.velocity;

			Vector2 dustUnit = (GetOffset(Projectile.rotation, player) * 2.5f).RotatedBy(Main.rand.NextFloat(-1, 1)) * 0.03f;
			Vector2 pulseUnit = GetOffset(Projectile.rotation, player) * 2.5f * 0.03f;

			Vector2 dustOffset = player.Center + (GetOffset(Projectile.rotation, player) * (5f + 3 * (float)Math.Sqrt(Projectile.localAI[0] / 100f))) + player.velocity;
			Color color = Color.Lerp(new Color(35, 57, 222), new Color(140, 238, 255), (float)Math.Sqrt(Projectile.localAI[0] / 100f));
			Vector2 spawnPos = dustOffset + (pulseUnit * 30);

			if (player.channel && !firing)
			{
				if (Projectile.localAI[0] < 100)
				{
					maxCounter++;
					Projectile.localAI[0]++;

					var dust = Dust.NewDustPerfect(dustOffset + (dustUnit * 30), 226);
					dust.velocity = Vector2.Zero - (dustUnit * 4);
					dust.noGravity = true;
					dust.scale = (float)Math.Sqrt(Projectile.localAI[0] / 100f);
					if (Projectile.localAI[0] % 10 == 9)
					{
						ParticleHandler.SpawnParticle(new PulseCircle(spawnPos, color * 0.4f, (.5f + .8f * (float)Math.Sqrt(Projectile.localAI[0] / 100f)) * 100, 20, PulseCircle.MovementType.InwardsQuadratic)
						{
							Angle = player.itemRotation,
							ZRotation = 0.6f,
							RingColor = color,
							Velocity = Vector2.Zero - (pulseUnit * 1.5f)
						});
						SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/EnergyCharge1") with { PitchVariance = 0.1f, Volume = 0.275f }, player.Center);
					}
				}

				Projectile.rotation = GetOffset(Projectile.rotation, player).RotatedBy(Main.rand.NextFloat(0 - ((float)Math.Sqrt(Projectile.localAI[0]) / 300f), (float)Math.Sqrt(Projectile.localAI[0]) / 300f)).ToRotation();
			}
			else
			{
				firing = true;

				if (Projectile.localAI[0] > 0)
				{
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/EnergyBlastMedium") with { PitchVariance = 0.1f, Volume = 0.275f }, player.Center);
					ParticleHandler.SpawnParticle(new PulseCircle(dustOffset + (pulseUnit * 10), color * 0.4f, (.5f + .8f * (float)Math.Sqrt(Projectile.localAI[0] / 100f)) * 100, 20, PulseCircle.MovementType.OutwardsQuadratic)
					{
						Angle = player.itemRotation,
						ZRotation = 0.6f,
						RingColor = color,
						Velocity = Vector2.Zero - (pulseUnit * 1.5f)
					});
				}
				while (Projectile.localAI[0] >= 0)
				{
					if (Projectile.localAI[0] > 90)
					{
						for (int i = 0; i < 20; i++)
						{
							var dust = Dust.NewDustPerfect(dustOffset + (dustUnit * 30), 226);
							dust.velocity = Vector2.Zero + (Vector2.Normalize(Main.MouseWorld - (player.Center - new Vector2(4, 4))).RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(2f, 6f) * 3f);
							dust.noGravity = true;
							dust.scale = (float)Math.Sqrt(Projectile.localAI[0] / 100f);
						}
					}

					Projectile.localAI[0] -= 10;
					Vector2 toShoot = GetOffset(Projectile.rotation, player).RotatedBy(Main.rand.NextFloat(0 - ((float)Math.Sqrt(200 - maxCounter) / 40f), (float)Math.Sqrt(200 - maxCounter) / 40f));

					Projectile.velocity = toShoot;
					toShoot *= 2.3f;
					toShoot *= (float)Math.Pow(maxCounter, 0.18);

					if (player.whoAmI == Main.myPlayer)
					{
						int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center + (GetOffset(Projectile.rotation, player) * 4), toShoot * new Vector2(Main.rand.NextFloat(.6f, 1.3f), Main.rand.NextFloat(0.6f, 1.2f)), ModContent.ProjectileType<HeavenfleetStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

						if (Main.netMode != NetmodeID.SinglePlayer)
							NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
					}
					player.GetModPlayer<MyPlayer>().Shake += 1;
				}

				Projectile.active = false;
				Projectile.netUpdate = true;
			}

			player.itemRotation = MathHelper.WrapAngle(Projectile.rotation + ((Projectile.spriteDirection < 0) ? MathHelper.Pi : 0));
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			Vector2 pos = player.Center + GetOffset(Projectile.rotation, player) * 3f + player.velocity;
			SpriteEffects effects = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			float rotation = Projectile.rotation + ((effects == SpriteEffects.FlipHorizontally) ? MathHelper.Pi : 0);

			if (Projectile.localAI[0] == 100)
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + (0.225f * k);

					if (num107 < 0)
						num107 = 0;

					Color color = Color.White * 0.75f * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

					Texture2D tex = ModContent.Request<Texture2D>("SpiritMod/Items/Sets/GunsMisc/HeavenFleet/HeavenFleet_Glow").Value;

					Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, color, rotation, tex.Size() / 2, num107, effects, default);
				}
			}
			return true;
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			Player player = Main.player[Projectile.owner];

			Vector2 pos = player.Center + GetOffset(Projectile.rotation, player) * 2.5f + player.velocity;
			SpriteEffects effects = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			float rotation = Projectile.rotation + ((effects == SpriteEffects.FlipHorizontally) ? MathHelper.Pi : 0);

			if (Projectile.localAI[0] == 100)
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Color color = Color.White * 0.75f * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
					Texture2D tex = ModContent.Request<Texture2D>("SpiritMod/Items/Sets/GunsMisc/HeavenFleet/HeavenFleet_Lights").Value;

					spriteBatch.Draw(tex, pos - screenPos, null, color, rotation, tex.Size() / 2, Projectile.scale, effects, default);
				}
			}
		}
	}
}