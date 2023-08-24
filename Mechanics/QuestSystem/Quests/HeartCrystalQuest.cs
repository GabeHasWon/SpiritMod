using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ID;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class HeartCrystalQuest : Quest
    {
		public override int QuestClientID => NPCID.Guide;
		public override int Difficulty => 2;
		public override string QuestCategory => "Forager";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			((int)Terraria.ID.ItemID.GoldCoin, 1)
		};

		public override void OnQuestComplete()
		{
            bool showUnlocks = true;
			QuestManager.UnlockQuest<DecrepitDepths>(showUnlocks);
			QuestManager.UnlockQuest<SkyHigh>(showUnlocks);
			QuestManager.UnlockQuest<ItsNoSalmon>(showUnlocks);
			QuestManager.UnlockQuest<ManicMage>(showUnlocks);
			QuestManager.UnlockQuest<FriendSafari>(showUnlocks);
			QuestManager.UnlockQuest<BreakingAndEntering>(showUnlocks);
			base.OnQuestComplete();
		}

		private HeartCrystalQuest()
        {
            _tasks.AddTask(new RetrievalTask(ItemID.LifeCrystal, 1));
        }
    }
}