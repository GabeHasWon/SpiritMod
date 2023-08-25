using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
	public class CritterCaptureFloater : Quest
    {
		public override int Difficulty => 1;
		public override string QuestCategory => "Forager";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Weapon.Magic.LuminanceSeacone.LuminanceSeacone>(), 1),
			(ItemID.JellyfishNecklace, 1),
			(ItemID.SonarPotion, 3),
			(ItemID.SilverCoin, 55)
		};

		private CritterCaptureFloater()
        {
			TaskBuilder branch1 = new TaskBuilder();
			branch1.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.FloaterItem>(), 1, QuestManager.Localization("Capture")))
				  .AddTask(new GiveNPCTask(NPCID.Dryad, ModContent.ItemType<Items.Consumable.FloaterItem>(), 1, GetText("Thanks"), GetText("BringBack"), true, true));

			TaskBuilder branch3 = new TaskBuilder();
			branch3.AddParallelTasks(
				new SlayTask(new int[] { ModContent.NPCType<NPCs.Critters.Ocean.Floater>() }, 2),
				new RetrievalTask(ModContent.ItemType<Items.Consumable.Fish.RawFish>(), 1, QuestManager.Localization("Harvest")))
				.AddTask(new GiveNPCTask(NPCID.Angler, ModContent.ItemType<Items.Consumable.Fish.RawFish>(), 1, GetText("AnglerThanks"), GetText("ReturnAngler"), true, true, ModContent.ItemType<Items.Consumable.Fish.CrystalFish>()));

			TaskBuilder branch4 = new TaskBuilder();
			branch4.AddTask(new TalkNPCTask(NPCID.Dryad, GetText("UpsetDryad"), GetText("Tattle"), null, ModContent.ItemType<Items.Sets.FloatingItems.SunkenTreasure>()))
				   .AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.FloaterItem>(), 1, QuestManager.Localization("Capture")))
				   .AddTask(new GiveNPCTask(NPCID.Dryad, ModContent.ItemType<Items.Consumable.FloaterItem>(), 1, GetText("Thanks"), GetText("BringBack"), true, true));

			TaskBuilder branch2 = new TaskBuilder();
			branch2.AddTask(new TalkNPCTask(NPCID.Angler, GetText("HungryAngler"), GetText("AskAngler")))
				   .AddBranches(branch3, branch4);

			_tasks.AddBranches(branch1, branch2);
		}
	}
}