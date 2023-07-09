using SpiritMod.Items.Sets.ReefhunterSet;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestOcean : Quest
    {
        public override string QuestName => "Below the Waves";
		public override string QuestClient => "The Angler";
		public override string QuestDescription => "Do ya know why I love having you catch fish for me? I have the itch to see every darn fish on this planet before I kick the bucket. And I want you to feel that way, too! Take a dive and see how amazing the ocean can be. Grab me something off the ocean floor while you're there!";
		public override int Difficulty => 1;
		public override string QuestCategory => "Explorer";
		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Sets.CascadeSet.Coral_Catcher.Coral_Catcher>(), 1),
			(ModContent.ItemType<Items.Sets.FloatingItems.FishLure>(), 2),
			(ItemID.CopperCoin, 80)
		};

		public override void OnQuestComplete()
		{
			ModContent.GetInstance<QuestWorld>().AddQuestQueue(NPCID.Angler, QuestManager.GetQuest<AnglerStatueQuest>());
			base.OnQuestComplete();
		}

		private ExplorerQuestOcean()
		{
			_tasks.AddTask(new ExploreTask((Player player) => player.ZoneBeach && player.GetModPlayer<MyPlayer>().Submerged(30), 5000f, "the ocean depths"))
				.AddTask(new RetrievalTask(ModContent.ItemType<SulfurDeposit>(), 5, "", "Grab 5 Sulfur Deposits from hydrothermal vents"))
			    .AddTask(new GiveNPCTask(NPCID.Angler, ModContent.ItemType<SulfurDeposit>(), 5, "This smells...really bad! How do fish like this stuff? Why didn't you get me something cooler? Whatever...you can keep it. Go catch me some fish while you're out there next time!", "Return to the Angler", true, false));
		}
	}
}