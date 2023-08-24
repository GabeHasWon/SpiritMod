using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class SlayerQuestVultureMatriarch : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Slayer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Sets.Vulture_Matriarch.Vulture_Matriarch_Mask>(), 1),
			(Terraria.ID.ItemID.Sandgun, 1),
			(Terraria.ID.ItemID.SandBlock, 100),
			(Terraria.ID.ItemID.GoldCoin, 3)
		};

		private SlayerQuestVultureMatriarch()
        {
            _tasks.AddTask(new SlayTask(ModContent.NPCType<NPCs.Vulture_Matriarch.Vulture_Matriarch>(), 1, null, new QuestPoolData(0.3f, true)));
        }

		public override bool IsQuestPossible() => Main.hardMode;
	}
}