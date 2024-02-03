using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace SpiritMod.Tiles.Furniture.JadeDragonStatue
{
	[Sacrifice(1)]
	public class DragonStatueItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 34;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.value = Item.buyPrice(gold: 1);
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.createTile = ModContent.TileType<DragonStatue>();
		}
	}
}