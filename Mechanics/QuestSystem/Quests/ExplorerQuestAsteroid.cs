using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestAsteroid : Quest
    {
		public override int Difficulty => 2;
		public override string QuestCategory => "Explorer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Pins.PinRed>(), 1),
			(ModContent.ItemType<Items.Placeable.JumpPadItem>(), 2),
			(ModContent.ItemType<Items.Consumable.Quest.ExplorerScrollAsteroidFull>(), 1),
			(ModContent.ItemType<Items.Placeable.MusicBox.AsteroidBox>(), 1),
			(ModContent.ItemType<Items.Consumable.MapScroll>(), 3),
			(Terraria.ID.ItemID.GoldCoin, 3)
		};

		private ExplorerQuestAsteroid()
        {
            _tasks.AddTask(new ExploreTask((Player player) => player.ZoneAsteroid(), 5000f, GetText("Objective")));
        }
    }
}