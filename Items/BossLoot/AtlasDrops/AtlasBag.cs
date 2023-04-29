using SpiritMod.NPCs;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AtlasDrops
{
	public class AtlasBag : BossBagItem
	{
		internal override string BossName => "Atlas";

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			itemLoot.AddCommon<AtlasEye>();
			itemLoot.AddCommon<ArcaneGeyser>(1, 30, 46);
			itemLoot.AddOneFromOptions<Mountain, TitanboundBulwark, CragboundStaff, QuakeFist, Earthshatter>();
			AddBossItems<AtlasMask, Trophy8>(itemLoot, 12..17);
		}
	}
}
