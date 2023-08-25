using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestLuminous : Quest
    {
		public override int Difficulty => 1;
		public override string QuestCategory => "Explorer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Books.Book_LuminousArt>(), 1),
			(ModContent.ItemType<Items.Placeable.MusicBox.LuminousNightBox>(), 1),
			(ModContent.ItemType<Items.Consumable.MapScroll>(), 2),
			(Terraria.ID.ItemID.SilverCoin, 75)
		};

		private ExplorerQuestLuminous()
        {
             _tasks.AddTask(new ExploreTask((Player player) => player.ZoneBeach && MyWorld.luminousOcean, 1500f, GetText("Objective")));
        }
    }
}