using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.Items.Consumable.Quest;
using SpiritMod.Items.Weapon.Swung.Punching_Bag;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ZombieOriginQuest : Quest
    {
		public override string QuestClientOverride => "Unknown";
		public override int Difficulty => 3;
		public override string QuestCategory => "Slayer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Punching_Bag>(), 1),
			(ItemID.HerbBag, 3),
			(ModContent.ItemType<Items.Material.OldLeather>(), 8),
			(ItemID.GoldCoin, 2)
		};

		private ZombieOriginQuest()
        {
			int itemType = WorldGen.crimson ? ItemID.ViciousPowder : ItemID.VilePowder;
			int lureType = WorldGen.crimson ? ModContent.ItemType<WarlockLureCrimson>() : ModContent.ItemType<WarlockLureCorruption>();

            TaskBuilder branch1 = new TaskBuilder();
            branch1.AddTask(new TalkNPCTask(NPCID.Guide, GetText("ShowGuideDialogue"), GetText("TalkGuide")))
            	.AddTask(new RetrievalTask(ItemID.Book, 3))
            	.AddTask(new GiveNPCTask(NPCID.Guide, ItemID.Book, 3, GetText("GetLureGuide"), GetText("ReturnBooks")))
               	.AddTask(new RetrievalTask(ModContent.ItemType<ScientistLure>(), 1, QuestManager.Localization("Craft")))
				.AddTask(new SlayTask(ModContent.NPCType<NPCs.Dead_Scientist.Dead_Scientist>(), 1, GetText("KillScientist"), new QuestPoolData(0.75f, true))); 

            TaskBuilder branch2 = new TaskBuilder();
            branch2.AddTask(new TalkNPCTask(NPCID.Dryad, GetText("ShowDryadDialogue"), GetText("TalkDryad")))
           		.AddTask(new RetrievalTask(itemType, 5))
                .AddTask(new GiveNPCTask(NPCID.Dryad, itemType, 5, GetText("GetLureDryad"), GetText("ReturnPowder")))
               	.AddTask(new RetrievalTask(lureType, 1, QuestManager.Localization("Craft")))
				.AddTask(new SlayTask(ModContent.NPCType<NPCs.Undead_Warlock.Undead_Warlock>(), 1, GetText("KillWarlock"), new QuestPoolData(0.75f, true))); 
            _tasks.AddBranches(branch1, branch2);

        }
	}
}