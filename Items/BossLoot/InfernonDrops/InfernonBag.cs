using SpiritMod.Items.Sets.DonatorVanity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops
{
	public class InfernonBag : BossBagItem
	{
		internal override string BossName => "Infernon";

		public override void RightClick(Player player)
		{
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ItemID.GoldCoin, Main.rand.Next(2, 5));
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<HellsGaze>()); //expert drop
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<InfernalAppendage>(), Main.rand.Next(25, 36));

			int[] lootTable = {
				ModContent.ItemType<InfernalJavelin>(),
				ModContent.ItemType<DiabolicHorn>(),
				ModContent.ItemType<SevenSins>(),
				ModContent.ItemType<InfernalSword>(),
				ModContent.ItemType<InfernalStaff>(),
				ModContent.ItemType<InfernalShield>(),
				ModContent.ItemType<EyeOfTheInferno>()
			};
			int loot = Main.rand.Next(lootTable.Length);
			player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), lootTable[loot]);

			if (Main.rand.NextDouble() < 1d / 7)
				player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<InfernonMask>());
			if (Main.rand.NextDouble() < 1d / 10)
				player.QuickSpawnItem(player.GetSource_OpenItem(Item.type, "RightClick"), ModContent.ItemType<Trophy4>());

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