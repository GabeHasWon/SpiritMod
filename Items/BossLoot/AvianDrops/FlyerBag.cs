using SpiritMod.Items.BossLoot.AvianDrops.ApostleArmor;
using SpiritMod.Items.Sets.DonatorVanity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AvianDrops
{
	public class FlyerBag : BossBagItem
	{
		internal override string BossName => "Ancient Avian";

		public override void RightClick(Player player)
		{
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<AvianHook>());
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ItemID.GoldCoin, Main.rand.Next(5, 9));

			int[] lootTable = {
				ModContent.ItemType<TalonBlade>(),
				ModContent.ItemType<Talonginus>(),
				ModContent.ItemType<SoaringScapula>(),
				ModContent.ItemType<TalonPiercer>(),
				ModContent.ItemType<SkeletalonStaff>()
			};
			int loot = Main.rand.Next(lootTable.Length);
			int[] lootTable1 = {
				ModContent.ItemType<TalonHeaddress>(),
				ModContent.ItemType<TalonGarb>()
			};
			int loot1 = Main.rand.Next(lootTable1.Length);
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), lootTable[loot]);
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), lootTable1[loot1]);

			if (Main.rand.NextDouble() < 1d / 7)
				player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<FlierMask>());
			if (Main.rand.NextDouble() < 1d / 10)
				player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<Trophy2>());

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