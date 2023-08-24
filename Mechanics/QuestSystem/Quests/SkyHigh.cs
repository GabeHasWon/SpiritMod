using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class SkyHigh : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Forager";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(Terraria.ID.ItemID.GypsyRobe, 1),
			(Terraria.ID.ItemID.DynastyWood, 50),
			(Terraria.ID.ItemID.GoldCoin, 1),
			(ModContent.ItemType<Tiles.Furniture.JadeDragonStatue.DragonStatueItem>(), 1)
		};

		private SkyHigh()
        {
            _tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Weapon.Summon.JadeStaff>(), 1));
        }
    }
}