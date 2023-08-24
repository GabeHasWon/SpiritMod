using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestMushroom : Quest
    {
		public override int Difficulty => 2;
		public override string QuestCategory => "Explorer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Pins.PinBlue>(), 1),
			(Terraria.ID.ItemID.MushroomGrassSeeds, 5),
			(Terraria.ID.ItemID.GlowingMushroom, 10),
			(ModContent.ItemType<Items.Consumable.Quest.ExplorerScrollMushroomFull>(), 1),
			(ModContent.ItemType<Items.Consumable.MapScroll>(), 2),
			(Terraria.ID.ItemID.GoldCoin, 2)
		};

		private ExplorerQuestMushroom()
        {
            _tasks.AddTask(new ExploreTask((Player player) => player.ZoneGlowshroom, 5000f, "glowing mushroom fields"));
        }
   }
}