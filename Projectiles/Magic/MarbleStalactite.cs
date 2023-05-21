using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic
{
	public class MarbleStalactite : ModProjectile, ITrailProjectile
	{
		private readonly int lingerTime = 20;
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private bool Collided => Counter > 0;

		public void DoTrailCreation(TrailManager trailManager)
		{
			trailManager.CreateTrail(Projectile, new GradientTrail(Color.DarkGoldenrod with { A = 0 }, Color.Transparent), new RoundCap(), new DefaultTrailPosition(), 25, 200, new DefaultShader());
			trailManager.CreateTrail(Projectile, new StandardColorTrail(Color.PaleGoldenrod with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 50, 160, new ImageShader(Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_3", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, 0.05f, 1f, 1f));
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gilded Stalactite");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(30, 30);
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			if (!Collided)
			{
				for (int k = 0; k < 2; k++)
				{
					Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworkFountain_Yellow, (Projectile.velocity * Main.rand.NextFloat() * .2f).RotatedByRandom(1f), 0, default, .2f);
					dust.noGravity = true;
					dust.noLight = false;
				}

				if (WorldGen.SolidOrSlopedTile(Framing.GetTileSafely(Projectile.Center)))
				{
					ProjectileExtras.Explode(Projectile.whoAmI, 80, 80, delegate 
					{
						for (int j = 0; j < 12; j++)
						{
							if (j < 6)
							{
								Dust dust = Dust.NewDustPerfect(Projectile.position + new Vector2(Projectile.width * Main.rand.NextFloat(0.0f, 1.0f), Projectile.height / 2), DustID.FireworkFountain_Yellow, new Vector2(8f * Main.rand.NextFloat(-1.0f, 1.0f), 0), 0, default, 0.8f);
								dust.noGravity = true;
								dust.fadeIn = 1.2f;
							}
							else
							{
								Dust.NewDustPerfect(Projectile.position + new Vector2(Projectile.width * Main.rand.NextFloat(0.0f, 1.0f), Projectile.height / 2), DustID.FireworkFountain_Yellow, (-Projectile.velocity * Main.rand.NextFloat()).RotatedByRandom(MathHelper.Pi), 0, default, Main.rand.NextFloat(0.5f, 1.0f));

								if (!Main.dedServ)
								{
									Vector2 randomPos = Projectile.Center + Main.rand.NextVector2Unit() * 15;
									ParticleHandler.SpawnParticle(new ImpactLine(randomPos, Projectile.Center.DirectionTo(randomPos) * Main.rand.NextFloat(1.0f, 3.0f), Color.Lerp(Color.White, Color.Goldenrod, Main.rand.NextFloat(0.0f, 1.0f)), new Vector2(0.5f, Main.rand.NextFloat(0.5f, 1.0f)), 18)
									{ Rotation = randomPos.AngleTo(Projectile.Center) });
								}
							}
						}

						for (int i = 0; i < 18; i++)
						{
							int num = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, -1f, 100, default, 1.5f);
							Main.dust[num].velocity *= 1.2f;
						}

						for (int k = 0; k < 4; k++)
						{
							float scaleFactor = 0.4f;
							if (k > 3)
								scaleFactor = 0.8f;

							Gore gore = Main.gore[Gore.NewGore(Projectile.GetSource_Death("Explosion"), Projectile.position, default, Main.rand.Next(61, 64), 1f)];
							gore.velocity *= scaleFactor;

							float negateX = k > 2 ? -1 : 1;

							gore.velocity.X += 1f * negateX;
							gore.velocity.Y -= Main.rand.NextFloat() * 2f;
						}
					});

					Counter++;
					Projectile.netUpdate = true;
				}
			}
			else
			{
				if (++Counter >= lingerTime)
					Projectile.Kill();
			}

			Projectile.alpha = (int)MathHelper.Max(0, Projectile.alpha - (255 / 20));
			Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f;
		}

		public override bool ShouldUpdatePosition() => !Collided;

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			for (int k = 0; k < 6; k++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Marble, 2.5f * 1, -2.5f, 0, default, 0.27f);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Marble, 2.5f * 1, -2.5f, 0, default, 0.37f);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = TextureAssets.Projectile[Projectile.type].Size() / 2;
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}
	}

	public class MarblePortal : ModProjectile
	{
		public override string Texture => "SpiritMod/Textures/StardustPillarStar";
		public override void SetStaticDefaults() => DisplayName.SetDefault("Marble Portal");

		public const float MaxScale = 0.8f;

		public override void SetDefaults()
		{
			Projectile.Size = Vector2.One;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.scale = MaxScale * .5f;
			Projectile.timeLeft = 40;
		}

		public override bool? CanDamage() => false;

		public override void AI()
		{
			Projectile.rotation = 1.57f;

			float fadeTime = 20f;

			if (Projectile.timeLeft < fadeTime)
			{
				if ((Projectile.scale -= (float)(1f / fadeTime)) <= 0)
					Projectile.Kill();
			}
			else
			{
				Projectile.scale = Math.Min(Projectile.scale + (float)(1f / fadeTime), MaxScale);
			}

			if (Main.rand.NextBool(3) && Projectile.scale > .25f && !Main.dedServ)
				ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center + (Vector2.UnitX * Main.rand.NextFloat(-30.0f, 20.0f) * Projectile.scale), Vector2.UnitY * Main.rand.NextFloat() * 3f, Color.White, Color.Gold, Main.rand.NextFloat(0.1f, 0.35f), Main.rand.Next(15, 25)));
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			SpiritMod.ShaderDict["PortalShader"].Parameters["PortalNoise"].SetValue(Mod.Assets.Request<Texture2D>("Utilities/Noise/SpiralNoise").Value);
			SpiritMod.ShaderDict["PortalShader"].Parameters["DistortionNoise"].SetValue(Mod.Assets.Request<Texture2D>("Utilities/Noise/noise").Value);
			SpiritMod.ShaderDict["PortalShader"].Parameters["Timer"].SetValue((float)Main.timeForVisualEffects / 60f % MathHelper.TwoPi);
			SpiritMod.ShaderDict["PortalShader"].Parameters["DistortionStrength"].SetValue(0.1f);
			SpiritMod.ShaderDict["PortalShader"].Parameters["Rotation"].SetValue((float)Main.timeForVisualEffects / 40f % MathHelper.TwoPi);
			SpiritMod.ShaderDict["PortalShader"].CurrentTechnique.Passes[0].Apply();

			float opacitymod = (float)Math.Pow(Projectile.scale / MaxScale, 2);

			float numtodraw = 3;
			for (float i = 0; i < numtodraw; i++) //pulsating bloom type effect, draws multiple of the texture that grow in scale over time and fade out
			{
				float Timer = (Main.GlobalTimeWrappedHourly / 2 + i / numtodraw) % 1;
				float Opacity = 0.33f * (float)Math.Pow(Math.Sin(Timer * MathHelper.Pi), 3);
				float Scale = 0.6f * (Projectile.scale + (Timer * Projectile.scale));
				float Rotation = Main.GlobalTimeWrappedHourly / 2 * ((i % 2 == 0) ? -1 : 1) * (MathHelper.TwoPi * i / numtodraw);

				Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Gold * Opacity * opacitymod,
					Rotation, tex.Size() / 2, Scale, SpriteEffects.None, 0);
			}

			Vector2 ellipticalscale = new Vector2(Projectile.scale * 0.5f, Projectile.scale);
			Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.PaleGoldenrod * opacitymod, Projectile.rotation, tex.Size() / 2, //the main "body" of the portal
				ellipticalscale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(tex, Projectile.Center + (Vector2.UnitX * 3).RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.DarkGoldenrod * opacitymod,
				Projectile.rotation, tex.Size() / 2, ellipticalscale * 0.66f, SpriteEffects.None, 0);  //the center, moves slightly depending on direction for illusion of facing the player

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
			return false;
		}
	}
}
