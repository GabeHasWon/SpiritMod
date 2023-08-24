using Terraria.ModLoader;
using SpiritMod.NPCs.Reach;
using SpiritMod.NPCs.Town;
using SpiritMod.Mechanics.QuestSystem.Tasks;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
	public class SlayerQuestBriar : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Slayer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Accessory.FeralConcoction>(), 1),
			(ModContent.ItemType<Items.Sets.FloranSet.FloranCharm>(), 1),
			(ModContent.ItemType<Items.Sets.BriarDrops.EnchantedLeaf>(), 6),
			(Terraria.ID.ItemID.SilverCoin, 50)
		};

		private SlayerQuestBriar()
        {
            _tasks.AddTask(new SlayTask(new int[] { ModContent.NPCType<Reachman>(), ModContent.NPCType<ReachObserver>(), ModContent.NPCType<BlossomHound>(), ModContent.NPCType<ThornStalker>()}, 12));
        }

        public override void OnQuestComplete()
		{
            bool showUnlocks = true;
            QuestManager.UnlockQuest<SlayerQuestValkyrie>(showUnlocks);
			QuestManager.UnlockQuest<SlayerQuestDrBones>(showUnlocks);
			QuestManager.UnlockQuest<SlayerQuestNymph>(showUnlocks);
			QuestManager.UnlockQuest<SlayerQuestUGDesert>(showUnlocks);
			QuestManager.UnlockQuest<SlayerQuestCavern>(showUnlocks);

			ModContent.GetInstance<QuestWorld>().AddQuestQueue(ModContent.NPCType<Adventurer>(), QuestManager.GetQuest<ReturnToYourRoots>());

			base.OnQuestComplete();
        }
    }
}