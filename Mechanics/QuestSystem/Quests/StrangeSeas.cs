using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class StrangeSeas : Quest
    {
		public override int Difficulty => 4;
		public override string QuestCategory => "Main";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Armor.DiverSet.DiverHead>(), 1),
			(ModContent.ItemType<Items.Armor.DiverSet.DiverBody>(), 1),
			(ModContent.ItemType<Items.Armor.DiverSet.DiverLegs>(), 1),
			(ModContent.ItemType<Items.Sets.TideDrops.TribalScale>(), 3),
			(Terraria.ID.ItemID.GoldCoin, 6)
		};

		private StrangeSeas()
        {
            _tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.BlackPearl>(), 1))
				.AddTask(new SlayTask(ModContent.NPCType<NPCs.Tides.Rylheian>(), 1, "Slay the strange monster controlling the Tide"));
        }
    }
}