using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class RaidingTheStars : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Main";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Placeable.Furniture.StarplatePainting>(), 1),
			(ModContent.ItemType<Items.Sets.CoilSet.TechDrive>(), 7),
			(ModContent.ItemType<Items.Placeable.Tiles.ScrapItem>(), 50),
			(Terraria.ID.ItemID.GoldCoin, 4)
		};

		private RaidingTheStars()
        {
			_tasks.AddTask(new SlayTask(ModContent.NPCType<NPCs.Starfarer.CogTrapperHead>(), 2))
				.AddTask(new RetrievalTask(ModContent.ItemType<Items.Material.StarEnergy>(), 1, QuestManager.Localization("Craft")));
        }
    }
}