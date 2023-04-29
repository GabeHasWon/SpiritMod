using SpiritMod.Items.BossLoot.ScarabeusDrops.Khopesh;
using SpiritMod.Items.BossLoot.ScarabeusDrops.ScarabExpertDrop;
using SpiritMod.Items.BossLoot.ScarabeusDrops.AdornedBow;
using SpiritMod.Items.Equipment;
using Terraria.ModLoader;
using SpiritMod.NPCs;

namespace SpiritMod.Items.BossLoot.ScarabeusDrops
{
	public class BagOScarabs : BossBagItem
	{
		internal override string BossName => "Scarabeus";

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			itemLoot.AddCommon<ScarabPendant>();
			itemLoot.AddCommon<Chitin>(1, 24, 37);
			itemLoot.AddCommon<SandsOfTime>(3);
			itemLoot.AddOneFromOptions<ScarabBow, LocustCrook.LocustCrook, RoyalKhopesh, RadiantCane.RadiantCane>();
			AddBossItems<ScarabMask, Trophy1>(itemLoot, 2..4);
		}
	}
}