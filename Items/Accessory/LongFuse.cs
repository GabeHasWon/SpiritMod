using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory
{
	public class LongFuse : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Long Fuse");
			Tooltip.SetDefault("Explosives burn slower\nWorks while in the inventory");
		}

		public override void SetDefaults()
		{
			Item.Size = new Vector2(22, 28);
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 40, 0);
		}

		public override void UpdateInventory(Player player) => player.GetModPlayer<MyPlayer>().longFuse = true;
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) =>
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}
