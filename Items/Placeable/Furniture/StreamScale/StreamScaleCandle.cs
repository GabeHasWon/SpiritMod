using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using StreamScaleCandleTile = SpiritMod.Tiles.Furniture.StreamScale.StreamScaleCandleTile;

namespace SpiritMod.Items.Placeable.Furniture.StreamScale
{
	[Sacrifice(1)]
	public class StreamScaleCandle : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.value = 500;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<StreamScaleCandleTile>();
		}
	}
}