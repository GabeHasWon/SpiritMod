using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ExplorerQuestGranite : Quest
    {
		public override int Difficulty => 2;
		public override string QuestCategory => "Explorer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Armor.CapacitorSet.CapacitorHead>(), 1),
			(ModContent.ItemType<Items.Armor.CapacitorSet.CapacitorBody>(), 1),
			(ModContent.ItemType<Items.Armor.CapacitorSet.CapacitorLegs>(), 1),
			(ModContent.ItemType<Items.Consumable.Quest.ExplorerScrollGraniteFull>(), 1),
			(ModContent.ItemType<Items.Placeable.MusicBox.GraniteBox>(), 1),
			(ModContent.ItemType<Items.Consumable.MapScroll>(), 2),
			(Terraria.ID.ItemID.GoldCoin, 2)
		};

		private ExplorerQuestGranite()
        {
            _tasks.AddTask(new ExploreTask((Player player) => player.ZoneGranite, 5000f, GetText("Objective")));
        }
    }
}