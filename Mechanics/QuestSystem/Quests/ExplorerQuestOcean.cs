using SpiritMod.Items.Sets.ReefhunterSet;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestOcean : Quest
    {
		public override int QuestClientID => NPCID.Angler;
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
			_tasks.AddTask(new ExploreTask((Player player) => player.ZoneBeach && player.GetModPlayer<MyPlayer>().Submerged(30), 5000f, GetText("Ocean")))
				.AddTask(new RetrievalTask(ModContent.ItemType<SulfurDeposit>(), 5, null, GetText("GetSulfur")))
			    .AddTask(new GiveNPCTask(NPCID.Angler, ModContent.ItemType<SulfurDeposit>(), 5, GetText("Finish"), GetText("Return"), true, false));
		}
	}
}