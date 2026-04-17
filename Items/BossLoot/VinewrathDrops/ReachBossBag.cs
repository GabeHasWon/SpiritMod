using Terraria.ModLoader;
using SpiritMod.NPCs;

namespace SpiritMod.Items.BossLoot.VinewrathDrops;

public class ReachBossBag : BossBagItem
{
	internal override string BossName => "Vinewrath Bane";
	internal override bool Prehardmode => true;

	public override void ModifyItemLoot(ItemLoot itemLoot)
	{
		itemLoot.AddCommon<DeathRose>();
		itemLoot.AddOneFromOptions<ThornBow, SunbeamStaff, ReachVineStaff, ReachBossSword>();
		AddBossItems<ReachMask, Trophy5>(itemLoot, 3..7);
	}
}
