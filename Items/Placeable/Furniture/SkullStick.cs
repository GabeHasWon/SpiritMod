using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SkullStickTile = SpiritMod.Tiles.Ambient.SkullStick;

namespace SpiritMod.Items.Placeable.Furniture
{
	[Sacrifice(1)]
	public class SkullStick : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 28;
			Item.value = 500;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Green;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<SkullStickTile>();
		}
	}
}