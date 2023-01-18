using SpiritMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Items.Consumable
{
	public class ReachCrate : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thorny Crate");
			Tooltip.SetDefault("Right click to open");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.rare = ItemRarityID.Green;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.createTile = ModContent.TileType<ReachCrate_Tile>();
			Item.maxStack = 999;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.consumable = true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			string[] lootTable = { "AncientBark", "EnchantedLeaf", "BismiteCrystal" };
			int loot = Main.rand.Next(lootTable.Length);
			var source = player.GetSource_OpenItem(Item.type, "RightClick");
			player.QuickSpawnItem(source, Mod.Find<ModItem>(lootTable[loot]).Type, Main.rand.Next(3, 5));
			if (Main.rand.NextBool(2))
			{
				string[] lootTable1 = { "ReachBrooch", "ReachBoomerang", "ThornHook", "ReachChestMagic" };
				int loot1 = Main.rand.Next(lootTable1.Length);
				player.QuickSpawnItem(source, Mod.Find<ModItem>(lootTable1[loot1]).Type);
			}
			if (Main.rand.NextBool(4))
			{
				int stack = Main.rand.Next(5, 12);
				player.QuickSpawnItem(source, 73, stack);
			}

			if (Main.rand.NextBool(7))
			{
				int[] ores = { 12, 11, 13, 14, 699, 700, 701, 702 };
				int loot2 = Main.rand.Next(ores.Length);
				int oreamt = Main.rand.Next(10, 20);
				for (int j = 0; j < oreamt; j++)
					player.QuickSpawnItem(source, ores[loot2]);
			}
			if (Main.rand.NextBool(4))
			{
				int[] lootTable3 = { 2675, 2676 };
				int loot3 = Main.rand.Next(lootTable3.Length);
				int baitamt = Main.rand.Next(2, 6);
				for (int j = 0; j < baitamt; j++)
					player.QuickSpawnItem(source, lootTable3[loot3]);
			}

			if (Main.rand.NextBool(4))
			{
				int[] lootTable2 = { 19, 20, 21, 22, 703, 704, 705, 706 };
				int loot2 = Main.rand.Next(lootTable2.Length);
				int baramt = Main.rand.Next(10, 20);
				for (int j = 0; j < baramt; j++)
					player.QuickSpawnItem(source, lootTable2[loot2]);
			}
			if (Main.rand.NextBool(2))
			{
				int potions;
				potions = Main.rand.Next(new int[] { 288, 290, 292, 304, 298, 2322, 2323, 291, 2329 });
				int potamt = Main.rand.Next(2, 4);
				for (int j = 0; j < potamt; j++)
					player.QuickSpawnItem(source, potions);
			}

			if (Main.rand.NextBool(2))
			{
				int recoveryPots;
				recoveryPots = Main.rand.Next(new int[] { 188, 189 });
				int recamt = Main.rand.Next(5, 17);
				for (int j = 0; j < recamt; j++)
					player.QuickSpawnItem(source, recoveryPots);
			}
		}
	}
}