using SpiritMod.Tiles.Block;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BriarChestLoot
{
	[Sacrifice(1)]
	public class LivingElderbarkWand : ModItem
	{
		public override void SetStaticDefaults() => ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = 1;
			Item.tileWand = ModContent.ItemType<HuskstalkSet.AncientBark>();
			Item.consumable = false;
			Item.createTile = ModContent.TileType<LivingBriarWood>();
			Item.width = 36;
			Item.height = 36;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 20);
		}
	}
}
