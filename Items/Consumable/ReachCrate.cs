using SpiritMod.Items.Sets.BismiteSet;
using SpiritMod.Items.Sets.BriarChestLoot;
using SpiritMod.Items.Sets.BriarDrops;
using SpiritMod.Items.Sets.HuskstalkSet;
using SpiritMod.NPCs;
using SpiritMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable;

[Sacrifice(5)]
public class ReachCrate : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.rare = ItemRarityID.Green;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.createTile = ModContent.TileType<ReachCrate_Tile>();
		Item.maxStack = Item.CommonMaxStack;
		Item.autoReuse = true;
		Item.useAnimation = 15;
		Item.useTime = 10;
		Item.consumable = true;
	}

	public override bool CanRightClick() => true;

	public override void ModifyItemLoot(ItemLoot itemLoot)
	{
		itemLoot.Add(DropRules.LootPoolDrop.SameStack(3, 4, 1, 1, 1, ModContent.ItemType<AncientBark>(), ModContent.ItemType<EnchantedLeaf>(), ModContent.ItemType<BismiteCrystal>()));
		itemLoot.AddOneFromOptions<ReachBrooch, ReachBoomerang, ThornHook, ReachChestMagic>(2);
		itemLoot.AddOneFromOptions<LivingElderbarkWand, ThornyRod>(4);

		itemLoot.AddCommon(ItemID.GoldCoin, 1, 5, 12);
		itemLoot.Add(DropRules.LootPoolDrop.SameStack(10, 20, 1, 7, 1, ItemID.IronOre, ItemID.CopperOre, ItemID.GoldOre, ItemID.SilverOre, 
			ItemID.TinOre, ItemID.LeadOre, ItemID.TungstenOre, ItemID.PlatinumOre));
		itemLoot.Add(DropRules.LootPoolDrop.SameStack(2, 5, 1, 4, 1, ItemID.JourneymanBait, ItemID.MasterBait));
		itemLoot.Add(DropRules.LootPoolDrop.SameStack(10, 20, 1, 4, 1, ItemID.GoldBar, ItemID.IronBar, ItemID.CopperBar, ItemID.SilverBar, ItemID.TinBar, 
			ItemID.LeadBar, ItemID.TungstenBar, ItemID.PlatinumBar));
		itemLoot.Add(DropRules.LootPoolDrop.SameStack(2, 3, 1, 2, 1, ItemID.ObsidianSkinPotion, ItemID.SwiftnessPotion, ItemID.IronskinPotion, ItemID.HunterPotion, 
			ItemID.ShinePotion, ItemID.MiningPotion, ItemID.HeartreachPotion, ItemID.TrapsightPotion, ItemID.GillsPotion));
		itemLoot.Add(DropRules.LootPoolDrop.SameStack(5, 16, 1, 2, 1, ItemID.HealingPotion, ItemID.ManaPotion));
	}
}