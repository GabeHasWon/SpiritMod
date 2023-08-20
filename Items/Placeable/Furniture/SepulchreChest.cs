using SpiritMod.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture
{
	[Sacrifice(1)]
	public class SepulchreChest : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sepulchre Chest");
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 28;
			Item.value = Terraria.Item.buyPrice(0, 0, 40, 0);
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Green;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<SepulchreChestTile1>();
		}
	}
}