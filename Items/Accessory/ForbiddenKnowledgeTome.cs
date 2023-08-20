using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory
{
	public class ForbiddenKnowledgeTome : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Tome of Forbidden Knowledge");
			// Tooltip.SetDefault("Killing enemies releases homing spectral skulls");
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 30;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetSpiritPlayer().forbiddenTome = true;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
			=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}
