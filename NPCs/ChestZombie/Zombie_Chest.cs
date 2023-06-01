using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.ChestZombie
{
	public class Zombie_Chest : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 32;
			Item.maxStack = 999;
			Item.rare = ItemRarityID.White;
		}

		public override void SetStaticDefaults() => DisplayName.SetDefault("Chest");
		public override void GrabRange(Player player, ref int grabRange) => grabRange *= 0;
		public override bool ItemSpace(Player player) => true;

		public override bool OnPickup(Player player)
		{
			for (int i = 0; i < Item.stack; i++)
			{
				int drop = Main.rand.Next(11);

				if (drop == 0)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 280);
				if (drop == 1)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 281);
				if (drop == 2)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 284);
				if (drop == 3)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 282);
				if (drop == 4)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 279);
				if (drop == 5)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 285);
				if (drop == 6)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 953);
				if (drop == 7)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 946);
				if (drop == 8)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 3068);
				if (drop == 9)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 3069);
				if (drop == 10)
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 3084);

				if (Main.rand.NextBool(6))
				{
					int p = player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 3093);
					if (Main.rand.NextBool(5))
						Main.item[p].stack += Main.rand.Next(2);
					if (Main.rand.NextBool(10))
						Main.item[p].stack += Main.rand.Next(2);
				}

				if (Main.rand.NextBool(3))
				{
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 168, Main.rand.Next(3, 6));
				}

				if (Main.rand.NextBool(2))
				{
					int num3 = Main.rand.Next(2);
					int num4 = Main.rand.Next(8) + 3;
					if (num3 == 0)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), WorldGen.copperBar, num4);
					if (num3 == 1)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), WorldGen.ironBar, num4);
				}

				if (Main.rand.NextBool(2))
				{
					int num3 = Main.rand.Next(50, 101);
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 965, num3);
				}

				if (!Main.rand.NextBool(3))
				{
					int num3 = Main.rand.Next(2);
					int num4 = Main.rand.Next(26) + 25;
					if (num3 == 0)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 40, num4);
					if (num3 == 1)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 42, num4);
				}

				if (Main.rand.NextBool(2))
				{
					int num3 = Main.rand.Next(1);
					int num4 = Main.rand.Next(3) + 3;
					if (num3 == 0)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 28, num4);
				}

				if (!Main.rand.NextBool(3))
				{
					int p = player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 2350);
					Main.item[p].stack = Main.rand.Next(2, 5);
				}

				if (Main.rand.Next(3) > 0)
				{
					int num3 = Main.rand.Next(6);
					int num4 = Main.rand.Next(1, 3);
					if (num3 == 0)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 292, num4);
					if (num3 == 1)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 298, num4);
					if (num3 == 2)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 299, num4);
					if (num3 == 3)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 290, num4);
					if (num3 == 4)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 2322, num4);
					if (num3 == 5)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 2325, num4);
				}

				if (Main.rand.NextBool(2))
				{
					int num3 = Main.rand.Next(2);
					int num4 = Main.rand.Next(11) + 10;
					if (num3 == 0)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 8, num4);
					if (num3 == 1)
						player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 31, num4);
				}

				if (Main.rand.NextBool(2))
					player.QuickSpawnItem(player.GetSource_Loot("Pickup"), 72, Main.rand.Next(10, 30));
			}
			return false;
		}
	}
}
