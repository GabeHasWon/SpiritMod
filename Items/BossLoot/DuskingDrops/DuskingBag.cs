
using SpiritMod.Items.Sets.DonatorVanity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.DuskingDrops
{
	public class DuskingBag : BossBagItem
	{
		internal override string BossName => "Dusking";

		public override void RightClick(Player player)
		{
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ItemID.GoldCoin, Main.rand.Next(3, 7));
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<DuskPendant>());
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<DuskStone>(), Main.rand.Next(25, 36));

			int[] lootTable = {
				ModContent.ItemType<ShadowflameSword>(),
				ModContent.ItemType<UmbraStaff>(),
				ModContent.ItemType<ShadowSphere>(),
				ModContent.ItemType<Shadowmoor>()
			};
			int loot = Main.rand.Next(lootTable.Length);
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), lootTable[loot]);
			
			if (Main.rand.NextDouble() < 1d / 7)
				player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<DuskingMask>());
			if (Main.rand.NextDouble() < 1d / 10)
				player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<Trophy6>());

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
