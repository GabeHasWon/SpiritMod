using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
	public class UnidentifiedFloatingObjects : Quest
    {
		public override int Difficulty => 2;
		public override string QuestCategory => "Main";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Consumable.DistressJellyItem>(), 1),
			(Terraria.ID.ItemID.GoldCoin, 1)
		};

		private UnidentifiedFloatingObjects()
        {
            _tasks.AddTask(new ExploreTask((Player player) => (player.ZoneOverworldHeight || player.ZoneSkyHeight) && MyWorld.jellySky, 500f, "the strange Jelly Deluge"))
				.AddTask(new RetrievalTask(ModContent.ItemType<Items.Consumable.DreamlightJellyItem>(), 1, QuestManager.Localization("Catch")));
        }
    }
}