using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestBlueMoon : Quest
    {
		public override int Difficulty => 4;
		public override string QuestCategory => "Explorer";
		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Consumable.Potion.MoonJellyDonut>(), 5),
			(ModContent.ItemType<Items.Sets.SeraphSet.MoonStone>(), 6),
			(ModContent.ItemType<Items.Placeable.MusicBox.BlueMoonBox>(), 1),
			(Terraria.ID.ItemID.GoldCoin, 2)
		};


		private ExplorerQuestBlueMoon()
        {
			_tasks.AddTask(new ExploreTask((Player player) => player.ZoneOverworldHeight && MyWorld.blueMoon, 5000f, GetText("Objective")));
        }
    }
}