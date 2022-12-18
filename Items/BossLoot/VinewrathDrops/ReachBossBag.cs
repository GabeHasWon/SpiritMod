using SpiritMod.Items.Sets.DonatorVanity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.VinewrathDrops
{
	public class ReachBossBag : BossBagItem
	{
		internal override string BossName => "Vinewrath Bane";

		public override void RightClick(Player player)
		{
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ItemID.GoldCoin, Main.rand.Next(4, 8));
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<DeathRose>());

			int[] lootTable = {
				ModContent.ItemType<ThornBow>(),
				ModContent.ItemType<SunbeamStaff>(),
				ModContent.ItemType<ReachVineStaff>(),
				ModContent.ItemType<ReachBossSword>()
			};
			int loot = Main.rand.Next(lootTable.Length);
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), lootTable[loot]);

			if (Main.rand.NextDouble() < 1 / 7f)
				player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<ReachMask>());
			if (Main.rand.NextDouble() < 1 / 10f)
				player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<Trophy5>());

			int[] vanityTable = {
				ModContent.ItemType<WaasephiVanity>(),
				ModContent.ItemType<MeteorVanity>(),
				ModContent.ItemType<PixelatedFireballVanity>(),
				ModContent.ItemType<LightNovasVanity>()
			};
			int vanityloot = Main.rand.Next(vanityTable.Length);
			if (Main.rand.NextBool(20))
				player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), vanityTable[vanityloot]);
		}
	}
}
