using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ReturnToYourRoots : Quest
    {
		public override int Difficulty => 4;
		public override string QuestCategory => "Main";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Armor.ReachBoss.ReachBossHead>(), 1),
			(ModContent.ItemType<Items.Armor.ReachBoss.ReachBossBody>(), 1),
			(ModContent.ItemType<Items.Armor.ReachBoss.ReachBossLegs>(), 1),
			(ModContent.ItemType<Items.Placeable.Furniture.ReachPainting>(), 1),
			(Terraria.ID.ItemID.GoldCoin, 3)
		};

		private ReturnToYourRoots()
        {
            _tasks.AddTask(new SlayTask(ModContent.NPCType<NPCs.Boss.ReachBoss.ReachBoss1>(), 1, "Kill the Vinewrath Bane"));        
        }
    }
}