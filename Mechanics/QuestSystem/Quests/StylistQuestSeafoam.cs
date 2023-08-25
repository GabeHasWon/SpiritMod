using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.NPCs.Sea_Mandrake;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class StylistQuestSeafoam : Quest
    {
		public override int QuestClientID => NPCID.Stylist;
		public override int Difficulty => 2;
		public override string QuestCategory => "Forager";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Sets.DyesMisc.HairDye.SeafoamDye>(), 1),
			(ModContent.ItemType<Items.Material.HeartScale>(), 4),
			(ItemID.SilverCoin, 30)
		};

		public override void OnQuestComplete()
		{
			//bool showUnlocks = true;
			//QuestManager.UnlockQuest<StylistQuestCorrupt>(showUnlocks);
			//QuestManager.UnlockQuest<StylistQuestCrimson>(showUnlocks);
			base.OnQuestComplete();
		}

		private StylistQuestSeafoam()
        {
			_tasks.AddParallelTasks(new SlayTask(ModContent.NPCType<Sea_Mandrake>(), 1), new RetrievalTask(ModContent.ItemType<Items.Sets.MaterialsMisc.QuestItems.SeaMandrakeSac>(), 1, QuestManager.Localization("Harvest")))
				  .AddTask(new GiveNPCTask(NPCID.Stylist, ModContent.ItemType<Items.Sets.MaterialsMisc.QuestItems.SeaMandrakeSac>(), 1, GetText("Dialogue"), GetText("ReturnTo"), true, true));
		}
	}
}