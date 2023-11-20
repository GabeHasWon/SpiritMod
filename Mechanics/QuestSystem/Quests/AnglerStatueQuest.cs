using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.Items.Placeable.Furniture;
using Terraria.Localization;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class AnglerStatueQuest : Quest
    {
		public override int Difficulty => 1;
		public override string QuestCategory => "Forager";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ItemID.GoldCoin, 3),
			(ItemID.MasterBait, 3),
			(ModContent.ItemType<Items.Fishing.FisheyeGem>(), 1)
		};

		private AnglerStatueQuest()
        {
			TaskBuilder branch1 = new TaskBuilder();
			branch1.AddTask(new RetrievalTask(ItemID.RedSnapper, 3424, GetText("AppeaseLine")))
			       .AddTask(new GiveNPCTask(NPCID.Angler, ItemID.RedSnapper, 3424, LocalizedText.Empty, GetText("GiveToAngler"), true, true));
		
			TaskBuilder branch2 = new TaskBuilder();
			branch2.AddTask(new TalkNPCTask(NPCID.Guide, GetText("GuideCheckin"), GetText("AskGuide")))
				   .AddTask(new TalkNPCTask(NPCID.Angler, GetText("AnglerStatueDemand"), GetText("TalkLine")))
				   .AddTask(new RetrievalTask(ModContent.ItemType<GiantAnglerStatue>(), 1))
				   .AddTask(new GiveNPCTask(NPCID.Angler, ModContent.ItemType<GiantAnglerStatue>(), 1, GetText("CheerUp"), GetText("ShowAnglerCreation"), true, false, ItemID.FishermansGuide));

			TaskBuilder branch3 = new TaskBuilder();
			branch3.AddTask(new RetrievalTask(ModContent.ItemType<Items.Placeable.FishCrate>(), 3))
			      .AddTask(new GiveNPCTask(NPCID.Angler, ModContent.ItemType<Items.Placeable.FishCrate>(), 3, GetText("Defeated"), GetText("ShowAnglerCrates"), true, false))
				  .AddBranches(branch1, branch2);

			_tasks.AddBranches(branch3);
		}
	}
}