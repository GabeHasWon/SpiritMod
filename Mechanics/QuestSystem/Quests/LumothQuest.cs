using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
	public class LumothQuest : Quest
    {
		public override int QuestClientID => NPCID.Dryad;
		public override int Difficulty => 1;
		public override string QuestCategory => "Forager";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Books.Book_Lumoth>(), 1),
			(ItemID.ShinePotion, 3),
			(ItemID.SilverCoin, 35)
		};

		public override void OnQuestComplete()
		{
			QuestManager.UnlockQuest<CritterCaptureBlossmoon>(true);
			QuestManager.UnlockQuest<CritterCaptureFloater>(true);
			QuestManager.UnlockQuest<SporeSalvage>(true);

			base.OnQuestComplete();

		}
		private LumothQuest()
        {
			TaskBuilder branch1 = new TaskBuilder();
			branch1.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.LumothItem>(), 1, QuestManager.Localization("Capture")))
				.AddTask(new GiveNPCTask(NPCID.Dryad, ModContent.ItemType<Items.Consumable.LumothItem>(), 1, GetText("Thanks"), GetText("BringBack"), true, true));

			TaskBuilder branch2 = new TaskBuilder();
			branch2.AddTask(new TalkNPCTask(NPCID.Merchant, GetText("MerchantDialogue"), GetText("TalkMerchant")))
				.AddTask(new RetrievalTask(ModContent.ItemType<Items.Material.Brightbulb>(), 1, QuestManager.Localization("Harvest")))
				.AddTask(new GiveNPCTask(NPCID.Merchant, ModContent.ItemType<Items.Material.Brightbulb>(), 1,GetText("ThanksMerchant"), GetText("BringBackMerchant"), true, true, ItemID.IronCrate));

			_tasks.AddBranches(branch1, branch2);
		}
	}
}