using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.Items.Sets.MaterialsMisc.QuestItems;
using SpiritMod.NPCs.Town;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class IceDeityQuest : Quest
    {
		public override int QuestClientID => ModContent.NPCType<RuneWizard>();
		public override int Difficulty => 3;
		public override string QuestCategory => "Explorer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Placeable.IceSculpture.WinterbornSculpture>(), 3),
			(ModContent.ItemType<Items.Placeable.IceSculpture.IceDeitySculpture>(), 1),
			(ModContent.ItemType<Items.Armor.HunterArmor.SnowRangerHead>(), 1),
			(ModContent.ItemType<Items.Armor.HunterArmor.SnowRangerBody>(), 1),
			(ModContent.ItemType<Items.Armor.HunterArmor.SnowRangerLegs>(), 1),
			(ModContent.ItemType<Items.Accessory.FrostGiantBelt>(), 1),
			(ItemID.GoldCoin, 3)
		};

		private IceDeityQuest()
        {
			TaskBuilder branch2 = new TaskBuilder();
			branch2.AddTask(new SlayTask(new int[] { ModContent.NPCType<NPCs.Winterborn.WinterbornMelee>(), ModContent.NPCType<NPCs.WinterbornHerald.WinterbornMagic>() }, 5, GetText("KillWinterborn")))
				.AddTask(new RetrievalTask(ModContent.ItemType<IceDeityShard1>(), 1))
				.AddTask(new TalkNPCTask(ModContent.NPCType<RuneWizard>(), GetText("CuriousRelic"), GetText("ShowRelic")))
				.AddTask(new SlayTask(ModContent.NPCType<NPCs.CrystalDrifter.CrystalDrifter>(), 1, GetText("KillCrystalDrifter"), new QuestPoolData(0.65f, true)))
				.AddTask(new RetrievalTask(ModContent.ItemType<IceDeityShard2>(), 1))
				.AddParallelTasks(new RetrievalTask(ModContent.ItemType<Items.Sets.CryoliteSet.CryoliteBar>(), 8), new RetrievalTask(ModContent.ItemType<Items.Placeable.Tiles.CreepingIce>(), 25))
				.AddTask(new RetrievalTask(ModContent.ItemType<IceDeityShard3>(), 1))
				.AddTask(new GiveNPCTask(ModContent.NPCType<RuneWizard>(), new int[] { ModContent.ItemType<IceDeityShard1>(), ModContent.ItemType<IceDeityShard2>(), ModContent.ItemType<IceDeityShard3>() }, 
					new int[] { 1, 1, 1 }, GetText("Congrats"), GetText("GiveAllArtifacts"), true, true));

			TaskBuilder branch3 = new TaskBuilder();
			branch3.AddTask(new TalkNPCTask(ModContent.NPCType<NPCs.FrozenSouls.WintrySoul>(), GetText("FrozenSoulScene"), GetText("FindFrozenSoul"), new QuestPoolData(0.85f, true)))
				.AddTask(new RetrievalTask(ModContent.ItemType<IceDeityShard1>(), 1))
				.AddTask(new TalkNPCTask(ModContent.NPCType<RuneWizard>(), GetText("CuriousRelic"), GetText("ShowRelic")))
				.AddTask(new SlayTask(ModContent.NPCType<NPCs.CrystalDrifter.CrystalDrifter>(), 1, GetText("KillCrystalDrifter"), new QuestPoolData(0.65f, true)))
				.AddTask(new RetrievalTask(ModContent.ItemType<IceDeityShard2>(), 1))
				.AddParallelTasks(new RetrievalTask(ModContent.ItemType<Items.Sets.CryoliteSet.CryoliteBar>(), 8), new RetrievalTask(ModContent.ItemType<Items.Placeable.Tiles.CreepingIce>(), 25))
				.AddTask(new RetrievalTask(ModContent.ItemType<IceDeityShard3>(), 1))
				.AddTask(new GiveNPCTask(ModContent.NPCType<RuneWizard>(), new int[] { ModContent.ItemType<IceDeityShard1>(), ModContent.ItemType<IceDeityShard2>(), ModContent.ItemType<IceDeityShard3>() }, 
					new int[] { 1, 1, 1 }, GetText("Congrats"), GetText("GiveAllArtifacts"), true, true));

			TaskBuilder branch1 = new TaskBuilder();
			branch1.AddTask(new TalkNPCTask(NPCID.Dryad, GetText("DryadMyths"), GetText("AskMyths")))
				   .AddTask(new TalkNPCTask(ModContent.NPCType<Adventurer>(), GetText("AdventurerMyths"), GetText("AskAdventurer")))
				   .AddTask(new TalkNPCTask(ModContent.NPCType<RuneWizard>(), GetText("ReturnToDialogue"), GetText("ReturnTo")))
				   .AddBranches(branch2, branch3);

			_tasks.AddBranches(branch1);
		}
	}
}