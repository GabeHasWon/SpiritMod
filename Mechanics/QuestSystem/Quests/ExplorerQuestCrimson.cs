using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestCrimson : Quest
    {
		public override int QuestClientID => NPCID.Guide;
		public override int Difficulty => 2;
		public override string QuestCategory => "Explorer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			((int)ItemID.PurificationPowder, 10),
			(ItemID.SilverCoin, 60)
		};

		private ExplorerQuestCrimson()
        {
            _tasks.AddTask(new ExploreTask((Player player) => player.ZoneCrimson, 4000f, "the Crimson"));
        }

		public override bool IsQuestPossible() => WorldGen.crimson;

		public override void OnQuestComplete()
		{
            bool showUnlocks = true;
            QuestManager.UnlockQuest<ExplorerQuestMarble>(showUnlocks);
			QuestManager.UnlockQuest<ExplorerQuestGranite>(showUnlocks);
			QuestManager.UnlockQuest<ExplorerQuestAsteroid>(showUnlocks);
			QuestManager.UnlockQuest<ExplorerQuestHive>(showUnlocks);
			QuestManager.UnlockQuest<ExplorerQuestMushroom>(showUnlocks);
			QuestManager.UnlockQuest<ExplorerQuestAurora>(showUnlocks);
			QuestManager.UnlockQuest<ExplorerQuestLuminous>(showUnlocks);
			base.OnQuestComplete();
        }
    }
}