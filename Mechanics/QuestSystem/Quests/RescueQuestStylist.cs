using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class RescueQuestStylist : Quest
    {
		public override int QuestClientID => NPCID.Demolitionist;
		public override int Difficulty => 2;
		public override string QuestCategory => "Other";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Accessory.LongFuse>(), 1),
			(ItemID.ManaHairDye, 1),
			(ItemID.LifeHairDye, 1),			
			(ItemID.Dynamite, 5),
			(ItemID.SilverCoin, 75)
		};

		public override void OnQuestComplete()
		{
			ModContent.GetInstance<QuestWorld>().AddQuestQueue(NPCID.Stylist, QuestManager.GetQuest<StylistQuestSeafoam>());
			base.OnQuestComplete();
		}

		private RescueQuestStylist()
        {
            _tasks.AddTask(new TalkNPCTask(NPCID.Stylist, GetText("Dialogue"), GetText("Objective")));
        }
    }
}