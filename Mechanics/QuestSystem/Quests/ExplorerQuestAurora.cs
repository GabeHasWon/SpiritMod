using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestAurora : Quest
    {
		public override int Difficulty => 1;
		public override string QuestCategory => "Explorer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Consumable.Food.IceBerries>(), 4),
			(ModContent.ItemType<Items.Placeable.Furniture.Paintings.AdvPainting15>(), 1),
			(ModContent.ItemType<Items.Placeable.MusicBox.AuroraBox>(), 1),
			(ModContent.ItemType<Items.Consumable.MapScroll>(), 2),
			(Terraria.ID.ItemID.SilverCoin, 55)
		};

		private ExplorerQuestAurora()
        {
            _tasks.AddTask(new ExploreTask((Player player) => (player.ZoneSnow || player.ZoneSkyHeight) && MyWorld.aurora, 1500f, GetText("Objective")));
        }
    }
}