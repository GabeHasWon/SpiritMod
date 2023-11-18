using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace SpiritMod.NPCs
{
	public static class NPCLootHelper
	{
		public static void Add(this ILoot loot, params IItemDropRule[] rules)
		{
			foreach (var item in rules)
				loot.Add(item);
		}

		public static void AddCommon(this ILoot loot, int itemID, int chanceDenominator = 1, int minStack = 1, int maxStack = 1)
		{
			if (maxStack < minStack)
				maxStack = minStack;
			loot.Add(ItemDropRule.Common(itemID, chanceDenominator, minStack, maxStack));
		}

		//Non-generic extension methods
		//NPCLoot
		public static void AddFood(this ILoot loot, int itemID, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) => loot.Add(ItemDropRule.Food(itemID, chanceDenominator, minStack, maxStack));
		public static void AddOneFromOptions(this ILoot loot, int chanceDenominator, params int[] types) => loot.Add(ItemDropRule.OneFromOptions(chanceDenominator, types));
		public static void AddBossBag(this ILoot loot, int itemID) => loot.Add(ItemDropRule.BossBag(itemID));

		//LeadingConditionRule
		public static void AddFood(this LeadingConditionRule loot, int itemID, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) => loot.OnSuccess(ItemDropRule.Food(itemID, chanceDenominator, minStack, maxStack));
		public static void AddOneFromOptions(this LeadingConditionRule loot, int chanceDenominator, params int[] types) => loot.OnSuccess(ItemDropRule.OneFromOptions(chanceDenominator, types));
		public static void AddBossBag(this LeadingConditionRule loot, int itemID) => loot.OnSuccess(ItemDropRule.BossBag(itemID));

		//Generic extension methods
		//NPCLoot
		public static void AddCommon<T>(this ILoot loot, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) where T : ModItem => loot.AddCommon(ModContent.ItemType<T>(), chanceDenominator, minStack, maxStack);
		public static void AddFood<T>(this ILoot loot, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) where T : ModItem => loot.Add(ItemDropRule.Food(ModContent.ItemType<T>(), chanceDenominator, minStack, maxStack));
		public static void AddBossBag<T>(this ILoot loot) where T : ModItem => loot.Add(ItemDropRule.BossBag(ModContent.ItemType<T>()));
		public static void AddMasterModeCommonDrop<T>(this ILoot loot) where T : ModItem => loot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<T>()));
		public static void AddMasterModeDropOnAllPlayers<T>(this ILoot loot, int chanceDenominator = 1) where T : ModItem => loot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<T>(), chanceDenominator));

		public static void AddMasterModeRelicAndPet<TRelic, TPet>(this ILoot loot) where TRelic : ModItem where TPet : ModItem
		{
			loot.AddMasterModeCommonDrop<TRelic>();
			loot.AddMasterModeDropOnAllPlayers<TPet>(4);
		}
		
		//LeadingConditionRule
		public static void AddCommon<T>(this LeadingConditionRule loot, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) where T : ModItem => loot.OnSuccess(ItemDropRule.Common(ModContent.ItemType<T>(), chanceDenominator, minStack, maxStack));
		public static void AddFood<T>(this LeadingConditionRule loot, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) where T : ModItem => loot.OnSuccess(ItemDropRule.Food(ModContent.ItemType<T>(), chanceDenominator, minStack, maxStack));
		public static void AddBossBag<T>(this LeadingConditionRule loot) where T : ModItem => loot.OnSuccess(ItemDropRule.BossBag(ModContent.ItemType<T>()));
		public static void AddMasterModeCommonDrop<T>(this LeadingConditionRule loot) where T : ModItem => loot.OnSuccess(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<T>()));
		public static void AddMasterModeDropOnAllPlayers<T>(this LeadingConditionRule loot, int chanceDenominator = 1) where T : ModItem => loot.OnSuccess(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<T>(), chanceDenominator));

		//Shortcut for getting a leading condition rule
		public static LeadingConditionRule NightCondition(this ILoot loot) => new LeadingConditionRule(new DropRuleConditions.NotDay());

		//Uh on no params generics!!! - OneFromOptions generic impl.
		//NPCLoot
		public static void AddOneFromOptions<T1, T2>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem => loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>());
		public static void AddOneFromOptions<T1, T2, T3>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem => loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>());
		public static void AddOneFromOptions<T1, T2, T3, T4>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem
			=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>());
		public static void AddOneFromOptions<T1, T2, T3, T4, T5>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem
			=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>());
		public static void AddOneFromOptions<T1, T2, T3, T4, T5, T6>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem where T6 : ModItem
			=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>(), ModContent.ItemType<T6>());
		public static void AddOneFromOptions<T1, T2, T3, T4, T5, T6, T7>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem where T6 : ModItem where T7 : ModItem
			=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>(), ModContent.ItemType<T6>(), ModContent.ItemType<T7>());

		//LeadingConditionRule
		public static void AddOneFromOptions<T1, T2>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem => loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>());
		public static void AddOneFromOptions<T1, T2, T3>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem => loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>());
		public static void AddOneFromOptions<T1, T2, T3, T4>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem
			=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>());
		public static void AddOneFromOptions<T1, T2, T3, T4, T5>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem
			=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>());
		public static void AddOneFromOptions<T1, T2, T3, T4, T5, T6>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem where T6 : ModItem
			=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>(), ModContent.ItemType<T6>());
		public static void AddOneFromOptions<T1, T2, T3, T4, T5, T6, T7>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem where T6 : ModItem where T7 : ModItem
			=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>(), ModContent.ItemType<T6>(), ModContent.ItemType<T7>());


	}
}
