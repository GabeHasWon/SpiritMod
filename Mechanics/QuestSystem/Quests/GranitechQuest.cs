using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
	public class GranitechQuest : Quest
	{
		public override int QuestClientID => NPCID.Mechanic;
		public override int Difficulty => 3;
		public override string QuestCategory => "Main";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Accessory.Rangefinder.Rangefinder>(), 1),
			(ModContent.ItemType<Items.Sets.GranitechSet.GranitechMaterial>(), 10),
			(Terraria.ID.ItemID.Wire, 100),
			(Terraria.ID.ItemID.GoldCoin, 3)
		};

		private GranitechQuest()
		{
			_tasks.AddParallelTasks(new SlayTask(ModContent.NPCType<NPCs.GraniTech.GraniteSentry>(), 3),
				new RetrievalTask(ModContent.ItemType<Items.Sets.GranitechSet.GranitechMaterial>(), 10, null))
				.AddTask(new GiveNPCTask(NPCID.Mechanic, ModContent.ItemType<Items.Sets.GranitechSet.GranitechMaterial>(), 10, GetText("Thanks"), GetText("BringBack"), true, false, ModContent.ItemType<NPCs.Town.Oracle.OracleScripture>()));
		}
	}
}