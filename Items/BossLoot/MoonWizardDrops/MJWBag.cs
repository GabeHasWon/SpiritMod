using SpiritMod.Items.BossLoot.MoonWizardDrops.JellynautHelmet;
using Terraria.ModLoader;
using SpiritMod.Items.Consumable;
using SpiritMod.NPCs;
using Terraria.GameContent.ItemDropRules;

namespace SpiritMod.Items.BossLoot.MoonWizardDrops;

public class MJWBag : BossBagItem
{
	internal override string BossName => "Moon Jelly Wizard";

	public override void ModifyItemLoot(ItemLoot itemLoot)
	{
		itemLoot.AddCommon<Cornucopion>();

		var opt = ItemDropRule.Common(ModContent.ItemType<Moonshot>(), 4); //Try dropping moonshot,
		opt.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TinyLunazoaItem>(), 1, 14, 20)); //if it drops, drop more tiny lunazoas, If not, drop another item.
		opt.OnFailedRoll(ItemDropRule.OneFromOptions(1, ModContent.ItemType<NautilusClub>(), ModContent.ItemType<JellynautBubble>(), ModContent.ItemType<MoonjellySummonStaff>()));
		itemLoot.Add(opt);

		itemLoot.AddCommon<TinyLunazoaItem>(1, 10, 12);
		AddBossItems<MJWMask, MJWTrophy>(itemLoot, 4..7);
	}
}
