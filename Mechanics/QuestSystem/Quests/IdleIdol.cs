using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
	public class IdleIdol : Quest
    {
		public override int Difficulty => 2;
		public override string QuestCategory => "Main";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Consumable.ScarabIdol>(), 1),
			(Terraria.ID.ItemID.Topaz, 2),
			(Terraria.ID.ItemID.Sapphire, 2),
			(Terraria.ID.ItemID.PharaohsMask, 1),
			(Terraria.ID.ItemID.PharaohsRobe, 1),
			(Terraria.ID.ItemID.GoldCoin, 1)
		};

		public IdleIdol()
        {
            _tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.Quest.ScarabIdolQuest>(), 1))
				  .AddTask(new GiveNPCTask(ModContent.NPCType<NPCs.Town.Adventurer>(), new int[] {ModContent.ItemType<Items.Consumable.Quest.ScarabIdolQuest>()}, new int[] { 1 }, GetText("Warning"), GetText("ReturnToAdventurer"), true));
        }

        public override void OnQuestComplete()
		{
			QuestManager.UnlockQuest<SinisterSands>(true);

			base.OnQuestComplete();
		}
    }
}