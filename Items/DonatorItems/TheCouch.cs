using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.DonatorItems
{
	public class TheCouch : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 52;
			Item.height = 30;
			Item.value = 50000;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.LightRed;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Tiles.Furniture.Donator.TheCouch>();
		}
	}
}