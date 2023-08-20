using SpiritMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.BriarDrops;

namespace SpiritMod.Items.Placeable.Furniture;

[Sacrifice(1)]
public class ForgottenKingStatueItem : ModItem
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Forgotten King Statue");
		// Tooltip.SetDefault("'A memorial of an ancient conquerer'");
	}

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 34;
		Item.value = Item.value = Item.sellPrice(0, 5, 0, 0);
		Item.rare = ItemRarityID.Orange;
		Item.maxStack = 99;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 10;
		Item.useAnimation = 15;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.createTile = ModContent.TileType<ForgottenKingStatue>();
	}
}