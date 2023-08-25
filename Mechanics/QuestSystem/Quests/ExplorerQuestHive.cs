using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestHive : Quest
    {
		public override int Difficulty => 2;
		public override string QuestCategory => "Explorer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Armor.BeekeeperSet.BeekeeperHead>(), 1),
			(ModContent.ItemType<Items.Armor.BeekeeperSet.BeekeeperBody>(), 1),
			(ModContent.ItemType<Items.Armor.BeekeeperSet.BeekeeperLegs>(), 1),
			(ModContent.ItemType<Items.Consumable.Quest.ExplorerScrollHiveFull>(), 1),
			(Terraria.ID.ItemID.BottledHoney, 10),
			(ModContent.ItemType<Items.Consumable.MapScroll>(), 2),
			(Terraria.ID.ItemID.GoldCoin, 1)
		};

		private ExplorerQuestHive()
        {
            _tasks.AddTask(new ExploreTask((Player player) => player.ZoneHive, 2000f, GetText("Objective")));
        }
    }
}