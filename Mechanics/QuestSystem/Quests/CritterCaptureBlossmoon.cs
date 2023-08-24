using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class CritterCaptureBlossmoon : Quest
    {
		public override int QuestClientID => NPCID.Dryad;
		public override int Difficulty => 1;
		public override string QuestCategory => "Forager";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Books.Book_Blossmoon>(), 1),
			(ModContent.ItemType<Tiles.Furniture.Critters.BlossomCage>(), 1),
			(ItemID.CalmingPotion, 3),
			(ItemID.SilverCoin, 55)
		};

		private CritterCaptureBlossmoon()
        {
            _tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.BlossmoonItem>(), 2, QuestManager.Localization("Capture")))
				  .AddTask(new GiveNPCTask(NPCID.Dryad, ModContent.ItemType<Items.Consumable.BlossmoonItem>(), 2, "Blossmoon actually have the ability to calm nearby monsters. I suspect they draw power from the moon to do so. It would be very beneficial for us all if we made sure all Blossmoon thrived. This is a good first step in that process!", "Bring the Blossmoon back to the Dryad", true, true));
		}
	}
}