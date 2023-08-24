using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;

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
			
			(ModContent.ItemType<Items.Sets.OlympiumSet.OlympiumToken>(), 5),
			(ModContent.ItemType<Items.Consumable.OliveBranch>(), 2),
			(ItemID.GoldCoin, 3)
		};

		private OlympiumQuest()
        {
			TaskBuilder path = new TaskBuilder();
			path.AddTask(new TalkNPCTask(ModContent.NPCType<NPCs.Town.Oracle.Oracle>(), "I've been expecting you, traveler. My visions spoke of a time when a hero would need some guidance. I can offer you powerful armaments in exchange for Olympium, the currency of the Gods. To find some, seek out and defeat enemies that have been blessed by the Gods themselves. I can give you my blessing if you would like to find more of these creatures, but you risk incurring the wrath of the Gods.'", "Find the mysterious woman in the Marble Caverns"))
				   .AddTask(new RetrievalTask(ModContent.ItemType<Items.Sets.OlympiumSet.OlympiumToken>(), 20))
				   .AddTask(new GiveNPCTask(ModContent.NPCType<NPCs.Town.Oracle.Oracle>(), ModContent.ItemType<Items.Sets.OlympiumSet.OlympiumToken>(), 20, "Ah, you've returned safe and sound! Wonderful. I'll exchange these tokens with you for some powerful weapons and equipment. And take this sacred parchment. If you need me, I will be by your side. Safe journey, hero.", "Return to the Oracle", true, false, ModContent.ItemType<NPCs.Town.Oracle.OracleScripture>()));

			_tasks.AddBranches(path);
		}
	}
}