using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.NPCs.Town.Oracle;
using SpiritMod.Items.Sets.OlympiumSet;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class OlympiumQuest : Quest
    {
		public override int QuestClientID => NPCID.Dryad;
		public override int Difficulty => 3;
		public override string QuestCategory => "Main";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{          
			
			(ModContent.ItemType<OlympiumToken>(), 5),
			(ModContent.ItemType<Items.Consumable.OliveBranch>(), 2),
			(ItemID.GoldCoin, 3)
		};

		private OlympiumQuest()
        {
			TaskBuilder path = new TaskBuilder();
			path.AddTask(new TalkNPCTask(ModContent.NPCType<Oracle>(), GetText("Dialogue"), GetText("Objective")))
				.AddTask(new RetrievalTask(ModContent.ItemType<OlympiumToken>(), 20))
				.AddTask(new GiveNPCTask(ModContent.NPCType<Oracle>(), ModContent.ItemType<OlympiumToken>(), 20, GetText("ReturnDialogue"), GetText("ReturnTo"), true, false, ModContent.ItemType<OracleScripture>()));

			_tasks.AddBranches(path);
		}
	}
}