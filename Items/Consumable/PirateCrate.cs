using SpiritMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	[Sacrifice(5)]
	public class PirateCrate : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.LightRed;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.maxStack = Item.CommonMaxStack;
			Item.createTile = ModContent.TileType<Tiles.Furniture.PirateCrate>();
			Item.useTime = Item.useAnimation = 20;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.noMelee = true;
			Item.autoReuse = false;
		}

		public override bool CanRightClick() => true;

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			int[] minorItem = new int[] //"Weighted" way of choosing a random, single item
			{
				ItemID.GoldRing, ItemID.GoldRing, ItemID.GoldRing, ItemID.GoldRing, ItemID.GoldRing, ItemID.GoldRing,
				ItemID.LuckyCoin, ItemID.LuckyCoin, ItemID.LuckyCoin,
				ItemID.CoinGun,
				ItemID.DiscountCard, ItemID.DiscountCard, ItemID.DiscountCard, ItemID.DiscountCard, ItemID.DiscountCard, ItemID.DiscountCard
			};

			itemLoot.AddOneFromOptions(1, minorItem);
			itemLoot.Add(DropRules.LootPoolDrop.SameStack(15, 30, 1, 4, 1, ItemID.GoldBar, ItemID.SilverBar, ItemID.TungstenBar, ItemID.PlatinumBar));
			itemLoot.Add(DropRules.LootPoolDrop.SameStack(15, 30, 1, 2, 1, ItemID.Ruby, ItemID.Emerald, ItemID.Topaz, ItemID.Amethyst, ItemID.Diamond, ItemID.Sapphire, 
				ItemID.Amber));
			itemLoot.AddCommon(ItemID.GoldCoin, 1, 10, 26);
		}
	}
}
