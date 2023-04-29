using SpiritMod.NPCs;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops
{
	public class InfernonBag : BossBagItem
	{
		internal override string BossName => "Infernon";

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			itemLoot.AddCommon<InfernalPact>();
			itemLoot.AddCommon<InfernalAppendage>(1, 25, 36);
			itemLoot.AddOneFromOptions<InfernalJavelin, DiabolicHorn, SevenSins, InfernalSword, InfernalStaff, InfernalShield, EyeOfTheInferno>();
			AddBossItems<InfernonMask, Trophy4>(itemLoot, 6..11);
		}
	}
}