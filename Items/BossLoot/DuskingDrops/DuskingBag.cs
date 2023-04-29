using SpiritMod.NPCs;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.DuskingDrops
{
	public class DuskingBag : BossBagItem
	{
		internal override string BossName => "Dusking";

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			itemLoot.AddCommon<DuskPendant>();
			itemLoot.AddCommon<DuskStone>(1, 25, 36);
			itemLoot.AddOneFromOptions<ShadowflameSword, UmbraStaff, ShadowSphere, Shadowmoor>(1);
			AddBossItems<DuskingMask, Trophy6>(itemLoot, 12..16);
		}
	}
}
