using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using SpiritMod.Items.Sets.BriarDrops;

namespace SpiritMod.Items.Sets.FloranSet
{
	public class FloranReacher : ModItem
	{
		public override void SetStaticDefaults() => Tooltip.SetDefault("Grants improved fishing power at nighttime");

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.GoldenFishingRod);
			Item.fishingPole = 24;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<FloranReacherProj>();
			Item.shootSpeed = 14f;
		}

		public override void UpdateInventory(Player player) => Item.fishingPole = 24 + (!Main.dayTime ? 14 : 0);

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ModContent.ItemType<FloranBar>(), 12);
			modRecipe.AddIngredient(ModContent.ItemType<EnchantedLeaf>(), 3);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}

	internal class FloranReacherProj : ModProjectile
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Bobber");

		public override void SetDefaults() => Projectile.CloneDefaults(ProjectileID.BobberGolden);

		public override bool PreDrawExtras()
		{
			Player player = Main.player[Projectile.owner];
			if (player.HeldItem.holdStyle == 0 || player.HeldItem.type != ModContent.ItemType<FloranReacher>())
				return false;

			Vector2 stringPos = player.MountedCenter + new Vector2((68 * player.direction) - 6, (34 * -player.gravDir) + player.gfxOffY) + (Projectile.Size / 2);
			stringPos = player.RotatedRelativePoint(stringPos + new Vector2(8f), true) - new Vector2(8f);

			Vector2 projPos = Projectile.Center - stringPos;

			Math.Sqrt((double)(projPos.X * projPos.X + projPos.Y * projPos.Y));

			bool flag2 = true;
			if (projPos == Vector2.Zero)
			{
				flag2 = false;
			}
			else
			{
				float projPosXY = (float)Math.Sqrt((double)(projPos.X * projPos.X + projPos.Y * projPos.Y));
				projPosXY = 12f / projPosXY;
				projPos *= projPosXY;
				stringPos -= projPos;
				projPos = Projectile.Center - stringPos;
			}
			while (flag2)
			{
				float num = 12f;
				float num2 = (float)Math.Sqrt((double)(projPos.X * projPos.X + projPos.Y * projPos.Y));
				float num3 = num2;
				if (float.IsNaN(num2) || float.IsNaN(num3))
				{
					flag2 = false;
				}
				else
				{
					if (num2 < 20f)
					{
						num = num2 - 8f;
						flag2 = false;
					}
					num2 = 12f / num2;
					projPos *= num2;
					stringPos += projPos;
					projPos = Projectile.Center - stringPos;

					if (num3 > 12f)
					{
						float num4 = 0.3f;
						float num5 = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y);
						if (num5 > 16f)
							num5 = 16f;

						num5 = 1f - num5 / 16f;
						num4 *= num5;
						num5 = num3 / 80f;
						if (num5 > 1f)
							num5 = 1f;

						num4 *= num5;
						if (num4 < 0f)
							num4 = 0f;

						num5 = 1f - Projectile.localAI[0] / 100f;
						num4 *= num5;
						if (projPos.Y > 0f)
						{
							projPos.Y *= 1f + num4;
							projPos.X *= 1f - num4;
						}
						else
						{
							num5 = Math.Abs(Projectile.velocity.X) / 3f;
							if (num5 > 1f)
								num5 = 1f;

							num5 -= 0.5f;
							num4 *= num5;
							if (num4 > 0f)
								num4 *= 2f;

							projPos.Y *= 1f + num4;
							projPos.X *= 1f - num4;
						}
					}
					float rotation = (float)Math.Atan2((double)projPos.Y, (double)projPos.X) - 1.57f;
					Color color = Lighting.GetColor((int)stringPos.X / 16, (int)(stringPos.Y / 16f), new Color(99, 110, 80, 100));

					Main.EntitySpriteDraw(TextureAssets.FishingLine.Value, new Vector2(stringPos.X - Main.screenPosition.X + (float)TextureAssets.FishingLine.Value.Width * 0.5f, stringPos.Y - Main.screenPosition.Y + (float)TextureAssets.FishingLine.Value.Height * 0.5f) - (Projectile.Size / 2), new Rectangle(0, 0, TextureAssets.FishingLine.Value.Width, (int)num), color, rotation, new Vector2((float)TextureAssets.FishingLine.Value.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0);
				}
			}
			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2, 0), Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}