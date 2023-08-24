using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class DecrepitDepths : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Forager";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Placeable.Furniture.SepulchrePotItem1>(), 4),
			(ModContent.ItemType<Items.Placeable.Tiles.SepulchreBrickTwoItem>(), 50),
			(Terraria.ID.ItemID.GoldCoin, 1)
		};

		private DecrepitDepths()
        {
            _tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Placeable.Furniture.SepulchreChest>(), 1));
        }
    }
}