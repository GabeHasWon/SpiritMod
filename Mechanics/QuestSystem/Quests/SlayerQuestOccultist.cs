using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
	public class SlayerQuestOccultist : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Slayer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Placeable.Furniture.OccultistMap>(), 1),
			(ModContent.ItemType<Items.Placeable.Furniture.SkullPile>(), 3),
			(ModContent.ItemType<Items.Sets.BloodcourtSet.DreamstrideEssence>(), 8),
			(ModContent.ItemType<Items.Weapon.Thrown.TargetBottle>(), 35),
			(Terraria.ID.ItemID.GoldCoin, 2)
		};

		private SlayerQuestOccultist()
        {
            _tasks.AddTask(new SlayTask(ModContent.NPCType<NPCs.Boss.Occultist.OccultistBoss>(), 1, null, null, true));
        }
    }
}