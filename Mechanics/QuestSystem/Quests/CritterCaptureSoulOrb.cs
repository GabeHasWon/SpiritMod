using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class CritterCaptureSoulOrb : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Forager";
		public override int QuestClientID => NPCID.Dryad;

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Books.Book_SpiritArt>(), 1),
			(ModContent.ItemType<Items.Sets.RunicSet.Rune>(), 5),
			(ModContent.ItemType<Items.Placeable.SoulSeeds>(), 3),
			(Terraria.ID.ItemID.GoldCoin, 3)
		};

		private CritterCaptureSoulOrb()
        {
			TaskBuilder branch1 = new TaskBuilder();
			branch1.AddTask(new GiveNPCTask(NPCID.Dryad, ModContent.ItemType<Items.Consumable.SoulOrbItem>(), 1, "These orbs seem to posses the souls of restless creatures. Perhaps we can bring them some peace in my sanctuary. You have done a great service for this soul and myself, traveller. Thank you.", "Bring the Soul Orb back to the Dryad", true, true));

			TaskBuilder branch2 = new TaskBuilder();
			branch2.AddTask(new RetrievalTask(ModContent.ItemType<Items.Material.SoulBloom>(), 1, QuestManager.Localization("OptionalGather")))
				   .AddTask(new GiveNPCTask(NPCID.Dryad, ModContent.ItemType<Items.Material.SoulBloom>(), 1, "These flowers are so otherworldly and mesmerizing. Thank you for bringing them to me. I hope that you can plant a field full of these wonderful flowers.", "Bring the Soul Orb and Soulbloom back to the Dryad", true, true, ModContent.ItemType<Items.Books.Book_Soulbloom>()))
				   .AddTask(new GiveNPCTask(NPCID.Dryad, ModContent.ItemType<Items.Consumable.SoulOrbItem>(), 1, "These orbs seem to posses the souls of restless creatures. Perhaps we can bring them some peace in my sanctuary. You have done a great service for this soul and myself, traveller. Thank you.", "Bring the Soul Orb and Soulbloom back to the Dryad", true, true));

			_tasks.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.SoulOrbItem>(), 1, null));
			_tasks.AddBranches(branch1, branch2);

		}
    }
}