using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
	public class BlastFromThePast : Quest
    {
		public override int Difficulty => 1;
		public override string QuestCategory => "Designer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Placeable.Furniture.Neon.Synthpalm>(), 1),
			(ModContent.ItemType<Items.Placeable.Furniture.Neon.VaporwaveItem>(), 1),
			(ModContent.ItemType<Items.Placeable.Furniture.Neon.ArcadeMachineItem>(), 2),
			(Terraria.ID.ItemID.SilverCoin, 75)
		};

		private BlastFromThePast()
        {
            _tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Placeable.Furniture.SynthwaveHeadItem>(), 1, QuestManager.Localization("Craft")));
        }
    }
}