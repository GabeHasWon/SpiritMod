using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestMarble : Quest
    {
		public override int Difficulty => 2;
		public override string QuestCategory => "Explorer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Armor.CenturionSet.CenturionHead>(), 1),
			(ModContent.ItemType<Items.Armor.CenturionSet.CenturionBody>(), 1),
			(ModContent.ItemType<Items.Armor.CenturionSet.CenturionLegs>(), 1),
			(ModContent.ItemType<Items.Consumable.Quest.ExplorerScrollMarbleFull>(), 1),
			(ModContent.ItemType<Items.Placeable.MusicBox.MarbleBox>(), 1),
			(ModContent.ItemType<Items.Consumable.MapScroll>(), 2),
			(Terraria.ID.ItemID.GoldCoin, 2)
		};

		private ExplorerQuestMarble()
        {
            _tasks.AddTask(new ExploreTask((Player player) => player.ZoneMarble, 5000f, "marble caverns"));
        }
    }
}