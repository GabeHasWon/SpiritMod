using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using StreamScaleBenchTile = SpiritMod.Tiles.Furniture.StreamScale.StreamScaleBenchTile;

namespace SpiritMod.Items.Placeable.Furniture.StreamScale
{
	[Sacrifice(1)]
	public class StreamScaleBench : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 18;
			Item.value = 500;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<StreamScaleBenchTile>();
		}
	}
}