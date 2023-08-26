using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using StreamScaleLanternTile = SpiritMod.Tiles.Furniture.StreamScale.StreamScaleLanternTile;

namespace SpiritMod.Items.Placeable.Furniture.StreamScale
{
	[Sacrifice(1)]
	public class StreamScaleLantern : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 28;
			Item.value = 200;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<StreamScaleLanternTile>();
		}
	}
}