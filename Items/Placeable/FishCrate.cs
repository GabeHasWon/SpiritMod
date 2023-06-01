using SpiritMod.Items.Armor.DiverSet;
using SpiritMod.Items.Consumable;
using SpiritMod.Items.Consumable.Fish;
using SpiritMod.NPCs;
using SpiritMod.Tiles.Furniture;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable
{
	[Sacrifice(10)]
	public class FishCrate : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Packing Crate");
			Tooltip.SetDefault("'A logo from a popular fishing company can be seen'\nRight click to open\nContains different types of fish");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.rare = ItemRarityID.Orange;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.createTile = ModContent.TileType<FishCrate_Tile>();
			Item.maxStack = 99;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.consumable = true;
		}

		public override bool CanRightClick() => true;

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			if (Item.wet)
			{
				gravity *= 0f;
				maxFallSpeed *= -.09f;
			}
			else
			{
				maxFallSpeed *= 1f;
				gravity *= 1f;
			}
		}

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			itemLoot.AddCommon<RawFish>(2);
			itemLoot.AddOneFromOptions<FloaterItem, LuvdiscItem>(4);
			itemLoot.Add(DropRules.LootPoolDrop.SameStack(3, 4, 1, 1, 1, ItemID.Shrimp, ItemID.Salmon, ItemID.Bass, ItemID.RedSnapper, ItemID.Trout));
			itemLoot.Add(DropRules.LootPoolDrop.SameStack(1, 2, 1, 4, 1, ItemID.Damselfish, ItemID.DoubleCod, ItemID.ArmoredCavefish, ItemID.FrostMinnow));
			itemLoot.AddOneFromOptions(27, ItemID.ReaverShark, ItemID.Swordfish, ItemID.SawtoothShark);
			itemLoot.AddOneFromOptions<DiverLegs, DiverBody, DiverHead>(14);
			itemLoot.Add(DropRules.LootPoolDrop.SameStack(9, 12, 1, 3, 1, ItemID.FrostDaggerfish, ItemID.BombFish));

			LeadingConditionRule isHardmode = new LeadingConditionRule(new Conditions.IsHardmode());
			isHardmode.OnSuccess(DropRules.LootPoolDrop.SameStack(1, 3, 1, 10, 1, ItemID.FlarefinKoi, ItemID.Obsidifish, ItemID.Prismite, ItemID.PrincessFish));
			itemLoot.Add(isHardmode);

			itemLoot.AddCommon(ItemID.SilverCoin, 3, 40, 91);
			itemLoot.AddCommon(ItemID.GoldCoin, 7, 2, 5);
		}
	}
}