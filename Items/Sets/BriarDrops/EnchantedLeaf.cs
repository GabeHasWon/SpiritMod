using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.HuskstalkSet;

namespace SpiritMod.Items.Sets.BriarDrops
{
	public class EnchantedLeaf : ModItem
	{
		public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AncientBark>();

		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.maxStack = 999;
			Item.value = 500;
			Item.rare = ItemRarityID.Blue;
			Item.alpha = 50;
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			for (int i = 0; i < 3; ++i)
			{
				float lerp = (float)Math.Sin(Main.timeForVisualEffects / (50f + (i * 5)) % Math.PI) * 1.3f;
				Color color = Item.GetAlpha(new Color(152, 250, 132, 0)) * lerp;

				spriteBatch.Draw(texture, position, null, color, 0, texture.Size() / 2, scale * lerp, SpriteEffects.None, 0);
			}
			return true;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Lighting.AddLight(new Vector2(Item.Center.X, Item.Center.Y), 81 * 0.001f, 194 * 0.001f, 58 * 0.001f);
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			for (int i = 0; i < 3; ++i)
			{
				float lerp = (float)Math.Sin(Main.timeForVisualEffects / (50f + (i * 5)) % Math.PI) * 1.3f;
				Color color = Item.GetAlpha(new Color(152, 250, 132, 0)) * lerp;

				spriteBatch.Draw(texture, Item.Center - Main.screenPosition, null, color, rotation, texture.Size() / 2, scale * lerp, SpriteEffects.None, 0);
			}
			return true;
		}
	}
}
