using SpiritMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable
{
	[Sacrifice(1)]
	public class SamuraiStatueItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Samurai Statue");
			// Tooltip.SetDefault("");
		}

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = 99;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<SamuraiStatue>();
			Item.width = 36;
			Item.height = 36;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(silver: 80);
		}
	}
}