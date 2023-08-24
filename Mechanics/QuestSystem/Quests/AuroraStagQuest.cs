using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class AuroraStagQuest : Quest
    {
		public override int Difficulty => 1;
		public override string QuestCategory => "Other";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			((int)ItemID.Holly, 1),
			(ItemID.HandWarmer, 1),
			(ItemID.ChristmasTree, 1),
			(ItemID.PineTreeBlock, 120),
			(ItemID.GoldCoin, 3)
		};

		private AuroraStagQuest()
        {
			_tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.Food.IceBerries>(), 1))
				  .AddTask(new RetrievalTask(ModContent.ItemType<Items.Equipment.AuroraSaddle.AuroraSaddle>(), 1, null, "Slowly approach an Aurora Stag and feed it Ice Berries"));
		}
	}
}