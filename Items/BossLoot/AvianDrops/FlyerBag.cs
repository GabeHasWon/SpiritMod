using SpiritMod.Items.BossLoot.AvianDrops.ApostleArmor;
using SpiritMod.NPCs;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AvianDrops
{
	public class FlyerBag : BossBagItem
	{
		internal override string BossName => "Ancient Avian";

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			itemLoot.AddCommon<AvianHook>();
			itemLoot.AddCommon(ItemID.GoldCoin, 1, 4, 6);
			itemLoot.AddOneFromOptions<TalonBlade, Talonginus, SoaringScapula, TalonPiercer, SkeletalonStaff>();
			itemLoot.AddOneFromOptions<TalonHeaddress, TalonGarb>();
			AddBossItems<FlierMask, Trophy2>(itemLoot, 5..9);
		}
	}
}