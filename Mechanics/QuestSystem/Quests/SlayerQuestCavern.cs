using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class SlayerQuestCavern : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Slayer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Weapon.Thrown.ClatterSpear>(), 1),
			(ModContent.ItemType<Items.Placeable.Furniture.WheezerPainting>(), 1),
			(ItemID.GoldCoin, 1)
		};

		private SlayerQuestCavern()
        {
            _tasks.AddTask(new SlayTask(new int[] { ModContent.NPCType<NPCs.Wheezer.Wheezer>(), ModContent.NPCType<NPCs.SporeWheezer.SporeWheezer>(), ModContent.NPCType<NPCs.CavernCrawler.CavernCrawler>(), 
				NPCID.GiantShelly, NPCID.Salamander, NPCID.Crawdad, NPCID.Salamander2, NPCID.Salamander3, NPCID.Salamander4, NPCID.Salamander5, NPCID.Salamander6, NPCID.Salamander7, NPCID.Salamander8, 
				NPCID.Salamander9, NPCID.GiantShelly2, NPCID.Crawdad2 }, 8, GetText("Objective")));
        }
    }
}