﻿using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.NPCs.Town;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
	public class BreakingAndEntering : Quest
    {
		public override int Difficulty => 2;
		public override string QuestCategory => "Main";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Sets.GamblerChestLoot.SilverChest>(), 1),
			(ModContent.ItemType<Items.Placeable.Furniture.TreasureChest>(), 2),
			(ModContent.ItemType<Items.Placeable.Furniture.PottedSakura>(), 3),
			(Terraria.ID.ItemID.GoldCoin, 1)
		};

		private BreakingAndEntering()
        {
			_tasks.AddTask(new TalkNPCTask(ModContent.NPCType<Gambler>(), GetText("Dialogue"), GetText("Objective")));
		}

		public override bool IsQuestPossible() => MyWorld.gennedTower;
	}
}