using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SpiritSet
{
	public class SpiritOre : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 12;
			Item.value = Item.buyPrice(0, 0, 50, 0);
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Pink;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<SpiritOreTile>();
		}
	}
}
