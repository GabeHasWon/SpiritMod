using SpiritMod.Tiles.Ambient.Ocean;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.PirateStuff
{
	[Sacrifice(1)]
	public class PirateChest : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 28;
			Item.value = 500;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.createTile = ModContent.TileType<OceanPirateChest>();
			Item.placeStyle = 0;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
		}
	}
}