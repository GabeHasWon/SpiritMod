﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic
{
	public class LeafProjReachChest : ModProjectile, ITrailProjectile
	{
		private bool released = false;

		public void DoTrailCreation(TrailManager tManager) => tManager.CreateTrail(Projectile, new StandardColorTrail(new Color(56, 194, 93)), 
			new RoundCap(), new DefaultTrailPosition(), 6f, 210f, new ImageShader(Mod.Assets.Request<Texture2D>("Textures/noise").Value, 0.2f, .4f, 1f));

		public override string Texture => "Terraria/Images/Projectile_206";

		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 5;
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.minionSlots = 0;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 180;
		}

		public override void AI()
		{
			if (++Projectile.frameCounter > 8)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
			}

			Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * .025f / 255f, (255 - Projectile.alpha) * .25f / 255f, (255 - Projectile.alpha) * .05f / 255f);
			Projectile.velocity.Y += Projectile.ai[0];
			if (Main.rand.NextBool(8))
				Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GrassBlades, 
					Projectile.velocity.X * .5f, Projectile.velocity.Y * .5f, 0, default, Main.rand.NextFloat(.4f, .7f));

			if (Main.myPlayer == Projectile.owner && Projectile.ai[0] <= 0f)
			{
				Projectile.netUpdate = true;

				if (Main.player[Projectile.owner].channel && !released)
				{
					float num2353 = 12f;
					Vector2 vector329 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
					float num2352 = (float)Main.mouseX + Main.screenPosition.X - vector329.X;
					float num2351 = (float)Main.mouseY + Main.screenPosition.Y - vector329.Y;

					if (Main.player[Projectile.owner].gravDir == -1f)
						num2351 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector329.Y;

					float num2350 = (float)Math.Sqrt((double)(num2352 * num2352 + num2351 * num2351));

					if (Projectile.ai[0] < 0f)
						Projectile.ai[0] += 1f;

					if (num2350 > num2353)
					{
						num2350 = num2353 / num2350;
						num2352 *= num2350;
						num2351 *= num2350;
						int num2345 = (int)(num2352 * 1000f);
						int num2344 = (int)(Projectile.velocity.X * 1000f);
						int num2343 = (int)(num2351 * 1000f);
						int num2342 = (int)(Projectile.velocity.Y * 1000f);

						if (num2345 != num2344 || num2343 != num2342)
							Projectile.netUpdate = true;

						if (Projectile.type == 491)
						{
							Vector2 value167 = new Vector2(num2352, num2351);
							Projectile.velocity = (Projectile.velocity * 4f + value167) / 5f;
						}
						else
						{
							Projectile.velocity.X = num2352;
							Projectile.velocity.Y = num2351;
						}
					}
					else
					{
						int num2341 = (int)(num2352 * 1000f);
						int num2340 = (int)(Projectile.velocity.X * 1000f);
						int num2339 = (int)(num2351 * 1000f);
						int num2338 = (int)(Projectile.velocity.Y * 1000f);
						if (num2341 != num2340 || num2339 != num2338)
							Projectile.netUpdate = true;

						Projectile.velocity.X = num2352;
						Projectile.velocity.Y = num2351;
					}
				}
				else if (Projectile.ai[0] <= 0f)
				{
					Projectile.netUpdate = true;

					float num2337 = 16f;
					Vector2 vector328 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
					float num2336 = (float)Main.mouseX + Main.screenPosition.X - vector328.X;
					float num2335 = (float)Main.mouseY + Main.screenPosition.Y - vector328.Y;

					if (Main.player[Projectile.owner].gravDir == -1f)
						num2335 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector328.Y;

					float num2334 = (float)Math.Sqrt((double)(num2336 * num2336 + num2335 * num2335));
					if (num2334 == 0f || Projectile.ai[0] < 0f)
					{
						vector328 = new Vector2(Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2), Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2));
						num2336 = Projectile.position.X + (float)Projectile.width * 0.5f - vector328.X;
						num2335 = Projectile.position.Y + (float)Projectile.height * 0.5f - vector328.Y;
						num2334 = (float)Math.Sqrt((double)(num2336 * num2336 + num2335 * num2335));
					}
					num2334 = num2337 / num2334;
					num2336 *= num2334;
					num2335 *= num2334;
					Projectile.velocity.X = num2336;
					Projectile.velocity.Y = num2335;

					if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
						Projectile.Kill();

					released = true;
					Projectile.ai[0] = 1f;
				}
			}

			if (Projectile.velocity != Vector2.Zero)
				Projectile.rotation = Projectile.velocity.ToRotation();
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
			Vector2 origin = new Vector2(texture.Width / 2, Projectile.height / 2);

			for (int k = 0; k < ProjectileID.Sets.TrailCacheLength[Type]; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, drawPos, frame, color, Projectile.oldRot[k], origin, Projectile.scale, SpriteEffects.None, 0f);
			}
			return false;
		}

		public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 100);

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.Center);
			for (int k = 0; k < 3; k++)
				Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GrassBlades, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
		}
	}
}