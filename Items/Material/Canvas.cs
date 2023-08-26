using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Material
{
	public class Canvas : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.value = Item.buyPrice(0, 0, 10, 0);
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = Item.CommonMaxStack;
			Item.createTile = ModContent.TileType<Tiles.Ambient.Canvas_Tile>();
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
		}
	}
}