using SpiritMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture
{
	[Sacrifice(1)]
	public class TreasureChestSmall : ModItem
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.TreasureChest.DisplayName");

		public override LocalizedText Tooltip => Language.GetText("Mods.SpiritMod.Items.TreasureChest.Tooltip");

		public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TreasureChestSmall2>();

		public override void SetDefaults()
		{
			Item.width = 48;
			Item.height = 24;
			Item.value = 850;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.buyPrice(0, 0, 90, 0);
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<TreasureChestTileSmall>();
		}
	}

	[Sacrifice(1)]
	public class TreasureChestSmall2 : TreasureChestSmall
	{
		public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TreasureChest>();

		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.createTile = ModContent.TileType<TreasureChestTileSmall>();
			Item.placeStyle = 1;
		}
	}
}