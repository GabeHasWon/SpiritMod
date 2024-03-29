﻿using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class SlayerQuestClown : Quest
    {
		public override int QuestClientID => NPCID.PartyGirl;
		public override int Difficulty => 3;
		public override string QuestCategory => "Slayer";

		//public override bool AnnounceRelocking => true;
		//public override bool LimitedUnlock => true;

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			((int)ItemID.Bananarang, 1),
			(ItemID.LightShard, 1),
			(ItemID.GoldCoin, 3)
		};

		private SlayerQuestClown()
        {
            _tasks.AddTask(new SlayTask(NPCID.Clown, 3, null, new QuestPoolData(0.2f, true, true, SpawnConditions)));
        }

		private bool SpawnConditions(NPCSpawnInfo arg) => Main.bloodMoon && arg.SpawnTileY < Main.worldSurface;

		public override bool IsQuestPossible() => Main.hardMode;
	}
}