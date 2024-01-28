using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.NPCs.Town;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class FirstAdventure : Quest
    {
		public override int Difficulty => 1;
		public override string QuestCategory => "Main";
		public override bool TutorialActivateButton => true;

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Consumable.MapScroll>(), 2),
			(ItemID.GoldCoin, 1)
		};

		private FirstAdventure()
        {
			_tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.Quest.DurasilkSheaf>(), 3))
				.AddParallelTasks(new RetrievalTask(ModContent.ItemType<Items.Armor.WayfarerSet.WayfarerHead>(), 1, QuestManager.Localization("Craft")), 
					new RetrievalTask(ModContent.ItemType<Items.Armor.WayfarerSet.WayfarerBody>(), 1, QuestManager.Localization("Craft")),
					new RetrievalTask(ModContent.ItemType<Items.Armor.WayfarerSet.WayfarerLegs>(), 1, QuestManager.Localization("Craft")));
		}

		public override void OnQuestComplete()
		{
			QuestManager.UnlockQuest<IdleIdol>(true);
			QuestManager.UnlockQuest<JellyfishHunter>(true);
			ModContent.GetInstance<QuestWorld>().AddQuestQueue(NPCID.Demolitionist, QuestManager.GetQuest<RescueQuestStylist>());
			ModContent.GetInstance<QuestWorld>().AddQuestQueue(NPCID.Dryad, QuestManager.GetQuest<LumothQuest>());
			//ModContent.GetInstance<QuestWorld>().AddQuestQueue(NPCID.TravellingMerchant, QuestManager.GetQuest<TravelingMerchantDesertQuest>());
			ModContent.GetInstance<QuestWorld>().AddQuestQueue(NPCID.Angler, QuestManager.GetQuest<ExplorerQuestOcean>());
			ModContent.GetInstance<QuestWorld>().AddQuestQueue(NPCID.Guide, QuestManager.GetQuest<HeartCrystalQuest>());
			ModContent.GetInstance<QuestWorld>().AddQuestQueue(NPCID.Guide, QuestManager.GetQuest<SlayerQuestScreechOwls>());
			ModContent.GetInstance<QuestWorld>().AddQuestQueue(ModContent.NPCType<Adventurer>(), QuestManager.GetQuest<SlayerQuestBriar>());
			ModContent.GetInstance<QuestWorld>().AddQuestQueue(ModContent.NPCType<Adventurer>(), QuestManager.GetQuest<BareNecessities>());

			if (WorldGen.crimson)
				ModContent.GetInstance<QuestWorld>().AddQuestQueue(NPCID.Guide, QuestManager.GetQuest<ExplorerQuestCrimson>());
			else
				ModContent.GetInstance<QuestWorld>().AddQuestQueue(NPCID.Guide, QuestManager.GetQuest<ExplorerQuestCorrupt>());

			QuestManager.SayInChat("Residents", Color.ForestGreen, true);
			QuestManager.SayInChat("ChatOpenBook", Color.GreenYellow, true);
			base.OnQuestComplete();
		}
	}
}