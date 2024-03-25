using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.Items.Equipment;
using SpiritMod.Items.Sets.DashSwordSubclass.AnimeSword;

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
			TaskBuilder fanBranch = new TaskBuilder().AddTask(new RetrievalTask(ModContent.ItemType<DynastyFan>(), 1));
			TaskBuilder bladeBranch = new TaskBuilder().AddTask(new RetrievalTask(ModContent.ItemType<AnimeSword>(), 1));
			TaskBuilder jadeBranch = new TaskBuilder().AddTask(new RetrievalTask(ModContent.ItemType<Items.Weapon.Summon.JadeStaff>(), 1));

			_tasks.AddBranches(fanBranch, bladeBranch, jadeBranch);
        }
    }
}