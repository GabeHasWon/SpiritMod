using SpiritMod.Items.Sets.DonatorVanity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AtlasDrops
{
	public class AtlasBag : BossBagItem
	{
		internal override string BossName => "Atlas";

		public override void RightClick(Player player)
		{
			var source = player.GetSource_OpenItem(Type);

			player.QuickSpawnItem(source, ItemID.GoldCoin, Main.rand.Next(7, 12));
			player.QuickSpawnItem(source, ModContent.ItemType<AtlasEye>());
			player.QuickSpawnItem(source, ModContent.ItemType<ArcaneGeyser>(), Main.rand.Next(30, 46));

			int[] lootTable = {
				ModContent.ItemType<Mountain>(),
				ModContent.ItemType<TitanboundBulwark>(),
				ModContent.ItemType<CragboundStaff>(),
				ModContent.ItemType<QuakeFist>(),
				ModContent.ItemType<Earthshatter>()
			};
			int loot = Main.rand.Next(lootTable.Length);
			player.QuickSpawnItem(source, lootTable[loot]);

			if (Main.rand.NextDouble() < 1 / 7f)
				player.QuickSpawnItem(source, ModContent.ItemType<AtlasMask>());
			if (Main.rand.NextDouble() < 1 / 10f)
				player.QuickSpawnItem(source, ModContent.ItemType<Trophy8>());

			int[] vanityTable = {
				ModContent.ItemType<WaasephiVanity>(),
				ModContent.ItemType<MeteorVanity>(),
				ModContent.ItemType<PixelatedFireballVanity>(),
				ModContent.ItemType<LightNovasVanity>()
			};
			int vanityloot = Main.rand.Next(vanityTable.Length);
			if (Main.rand.NextBool(20))
				player.QuickSpawnItem(source, vanityTable[vanityloot]);
		}
	}
}
