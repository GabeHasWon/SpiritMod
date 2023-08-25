using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.NPCs.Boss.MoonWizard;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class JellyfishHunter : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Main";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Consumable.Potion.MoonJelly>(), 8),
			(ModContent.ItemType<Items.Placeable.Tiles.SpaceJunkItem>(), 50),
			(Terraria.ID.ItemID.GoldCoin, 4)
		};

		private JellyfishHunter()
        {
            _tasks.AddTask(new SlayTask(ModContent.NPCType<MoonWizard>(), 1, GetText("Objective")));
        }
    }
}