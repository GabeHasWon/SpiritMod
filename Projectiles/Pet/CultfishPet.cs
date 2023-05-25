using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Pet
{
	public class CultfishPet : ModProjectile
	{
		private float frameCounter;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cultfish");
			Main.projFrames[Projectile.type] = 8;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.ZephyrFish);
			AIType = ProjectileID.ZephyrFish;
			Projectile.width = 40;
			Projectile.height = 60;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			player.GetModPlayer<PetPlayer>().PetFlag(Projectile);
			player.zephyrfish = false; // Relic from aiType

			Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.75f / 255f, (255 - Projectile.alpha) * 0.75f / 255f, (255 - Projectile.alpha) * 0f / 255f);

            if (Projectile.Distance(player.Center) > 1500)
                Projectile.position = player.position + new Vector2(Main.rand.Next(-125, 126), Main.rand.Next(-125, 126));

			if ((Projectile.localAI[0] = ++Projectile.localAI[0] % 10) == 0)
			{
				int num171 = 30;
				if ((Projectile.Center - Main.player[Main.myPlayer].Center).Length() < (float)(Main.screenWidth + num171 * 16))
				{
					int num172 = (int)Projectile.Center.X / 16;
					int num173 = (int)Projectile.Center.Y / 16;
					int num3;
					for (int num174 = num172 - num171; num174 <= num172 + num171; num174 = num3 + 1)
					{
						for (int num175 = num173 - num171; num175 <= num173 + num171; num175 = num3 + 1)
						{
							if (Main.rand.NextBool(4))
							{
								Vector2 vector16 = new Vector2((float)(num172 - num174), (float)(num173 - num175));
								if (vector16.Length() < (float)num171 && num174 > 0 && num174 < Main.maxTilesX - 1 && num175 > 0 && num175 < Main.maxTilesY - 1 && Main.tile[num174, num175] != null && Main.tile[num174, num175].HasTile)
								{
									bool flag3 = false;
									if (Main.tile[num174, num175].TileType == 185 && Main.tile[num174, num175].TileFrameY == 18)
									{
										if (Main.tile[num174, num175].TileFrameX >= 576 && Main.tile[num174, num175].TileFrameX <= 882)
										{
											flag3 = true;
										}
									}
									else if (Main.tile[num174, num175].TileType == 186 && Main.tile[num174, num175].TileFrameX >= 864 && Main.tile[num174, num175].TileFrameX <= 1170)
									{
										flag3 = true;
									}
									if (flag3 || Main.tileSpelunker[(int)Main.tile[num174, num175].TileType] || (Main.tileAlch[(int)Main.tile[num174, num175].TileType] && Main.tile[num174, num175].TileType != 82))
									{
										int num176 = Dust.NewDust(new Vector2((float)(num174 * 16), (float)(num175 * 16)), 16, 16, DustID.TreasureSparkle, 0f, 0f, 150, default, 0.3f);
										Main.dust[num176].fadeIn = 0.75f;
										Dust dust3 = Main.dust[num176];
										dust3.velocity *= 0.1f;
										Main.dust[num176].noLight = true;
									}
								}
							}
							num3 = num175;
						}
						num3 = num174;
					}
				}
			}

			Projectile.frame = (int)(frameCounter += .2f) % Main.projFrames[Type];
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}
	}
}