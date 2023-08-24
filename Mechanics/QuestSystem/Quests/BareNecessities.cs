using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class BareNecessities : Quest
    {
		public override int Difficulty => 1;
		public override string QuestCategory => "Designer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Tiles.Block.Ambient.RuinstoneItem>(), 50),
			(ModContent.ItemType<Tiles.Block.Ambient.FracturedStoneItem>(), 50),
			(ModContent.ItemType<Tiles.Block.Ambient.CragstoneItem>(), 50),
			(Terraria.ID.ItemID.SilverCoin, 25)
		};

 		public override void OnQuestComplete()
		{
            bool showUnlocks = true;
            QuestManager.UnlockQuest<AncestralWorship>(showUnlocks);
			QuestManager.UnlockQuest<StylishSetup>(showUnlocks);

            base.OnQuestComplete();
        }

		private BareNecessities()
        {
           _tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Placeable.Furniture.ForagerTableItem>(), 1, QuestManager.Localization("Craft")));
        }
    }
}