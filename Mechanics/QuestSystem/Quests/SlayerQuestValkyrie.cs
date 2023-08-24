using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class SlayerQuestValkyrie : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Slayer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(Terraria.ID.ItemID.SkyMill, 1),
			(ModContent.ItemType<Items.Consumable.ChaosPearl>(), 25),
			(ModContent.ItemType<Items.Weapon.Thrown.TargetBottle>(), 35),
			(Terraria.ID.ItemID.SilverCoin, 90)
		};

		private SlayerQuestValkyrie()
        {
            _tasks.AddTask(new SlayTask(ModContent.NPCType<NPCs.Valkyrie.Valkyrie>(), 1));
        }
	}
}