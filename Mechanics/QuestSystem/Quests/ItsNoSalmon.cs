using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class ItsNoSalmon : Quest
    {
		public override int Difficulty => 2;
		public override string QuestCategory => "Forager";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Accessory.KoiTotem>(), 1),
			(ModContent.ItemType<Items.Placeable.Furniture.FishingPainting>(), 1),
			(ItemID.Vine, 3),
			(ItemID.GoldCoin, 1)
		};

		private ItsNoSalmon()
        {
			TaskBuilder branch1 = new TaskBuilder();
			branch1.AddTask(new GiveNPCTask(ModContent.NPCType<NPCs.Town.Adventurer>(), ModContent.ItemType<Items.Consumable.Quest.HornetfishQuest>(), 1, GetText("ThanksAdventurer"), GetText("BringBack"), true, true, ModContent.ItemType<Items.Consumable.Fish.HoneySalmon>()));
			
			TaskBuilder branch2 = new TaskBuilder();
			branch2.AddTask(new GiveNPCTask(NPCID.Angler, ModContent.ItemType<Items.Consumable.Quest.HornetfishQuest>(), 1, GetText("GiveAngler"), GetText("BringBackAngler"), true, true, ItemID.MasterBait));

			_tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.Quest.HornetfishQuest>(), 1))
				  .AddBranches(branch1, branch2);
		}
	}
}