using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class SporeSalvage : Quest
    {
		public override int QuestClientID => NPCID.Dryad;
		public override int Difficulty => 2;
		public override string QuestCategory => "Forager";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Tiles.Furniture.Critters.VibeshroomJarItem>(), 1),
			(ModContent.ItemType<Items.Material.GlowRoot>(), 3),
			(Terraria.ID.ItemID.GlowingMushroom, 16),
			(Terraria.ID.ItemID.GoldCoin, 3)
		};

		private SporeSalvage()
        {
            _tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.VibeshroomItem>(), 1, QuestManager.Localization("Capture")));
        }
    }
}