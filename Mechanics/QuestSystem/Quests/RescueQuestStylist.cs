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
            _tasks.AddTask(new TalkNPCTask(NPCID.Stylist, "Don't go exploring with scissors, they said. You won't get trapped in a spider's web, they said! Who got you to rescue me, by the way? Oh, the Demolitionist? Tell him haircuts are on the house for life!", "Go to the spider caverns and rescue the captive"));
        }
    }
}