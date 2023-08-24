using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class StylishSetup : Quest
    {
		public override int QuestClientID => base.QuestClientID;
		public override int Difficulty => 1;
		public override string QuestCategory => "Designer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Placeable.Furniture.Neon.BlueNeonSign>(), 1),
			(ModContent.ItemType<Items.Placeable.Furniture.Neon.NeonPlantBlue>(), 1),
			(ModContent.ItemType<Items.Placeable.Tiles.NeonBlockBlueItem>(), 50),
			(Terraria.ID.ItemID.SilverCoin, 30)
		};

		private StylishSetup()
        {
            _tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Material.SynthMaterial>(), 1, QuestManager.Localization("Craft")));
        }

        public override void OnQuestComplete()
		{
            bool showUnlocks = true;
            QuestManager.UnlockQuest<BlastFromThePast>(showUnlocks);

            base.OnQuestComplete();
        }
    }
}