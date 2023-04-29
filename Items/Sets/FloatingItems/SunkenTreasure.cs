using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using SpiritMod.NPCs;
using Terraria.GameContent.ItemDropRules;

namespace SpiritMod.Items.Sets.FloatingItems
{
	public class SunkenTreasure : FloatingItem
	{
		public override float SpawnWeight => 0.001f;
		public override float Weight => base.Weight * 0.9f;
		public override float Bouyancy => base.Bouyancy * 1.08f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sunken Treasure");
			Tooltip.SetDefault("Right-click to open\n'Perhaps it is still full of valuable treasure''");
		}
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.LightRed;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.maxStack = 99;
			Item.createTile = ModContent.TileType<SunkenTreasureTile>();
			Item.useTime = Item.useAnimation = 20;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.noMelee = true;
			Item.autoReuse = false;
		}

		public override bool CanRightClick() => true;

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			int[] lootTable = new int[] //Another "weighted" loot table
			{
				ItemID.FishHook,
				ItemID.ShipsWheel, ItemID.ShipsWheel, ItemID.ShipsWheel,
				ItemID.TreasureMap,
				ItemID.CompassRose, ItemID.CompassRose,
				ItemID.ShipInABottle,
				ItemID.Sextant
			};

			itemLoot.AddOneFromOptions(3, lootTable);
			itemLoot.Add(DropRules.LootPoolDrop.SameStack(6, 10, 1, 4, 1, ItemID.GoldBar, ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar));
			itemLoot.Add(DropRules.LootPoolDrop.SameStack(5, 7, 1, 4, 1, ItemID.Ruby, ItemID.Emerald, ItemID.Topaz, ItemID.Amethyst, ItemID.Diamond, ItemID.Sapphire, ItemID.Amber));
			itemLoot.AddCommon<Weapon.Thrown.ExplosiveRum.ExplosiveRum>(1, 45, 71);

			var goldCoins = ItemDropRule.Common(ItemID.GoldCoin, 2, 1, 4);
			goldCoins.OnFailedRoll(ItemDropRule.Common(ItemID.Cobweb, 1, 8, 13));
			itemLoot.Add(goldCoins);
		}
	}

	public class SunkenTreasureTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileSpelunker[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Sunken Treasure");
			DustType = -1;
			AddMapEntry(new Color(133, 106, 56), name);
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<SunkenTreasure>());
	}
}