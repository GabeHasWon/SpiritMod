using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class SinisterSands : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Main";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Equipment.SandsOfTime>(), 1),
			(Terraria.ID.ItemID.GoldCoin, 5)
		};

		private SinisterSands()
        {
            _tasks.AddTask(new SlayTask(ModContent.NPCType<NPCs.Boss.Scarabeus.Scarabeus>(), 1, GetText("Objective")));        
        }
    }
}