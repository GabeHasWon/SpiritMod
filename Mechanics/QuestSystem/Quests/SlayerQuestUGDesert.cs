using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class SlayerQuestUGDesert : Quest
    {
		public override int Difficulty => 3;
		public override string QuestCategory => "Slayer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Armor.CowboySet.CowboyHead>(), 1),
			(ModContent.ItemType<Items.Armor.CowboySet.CowboyBody>(), 1),
			(ModContent.ItemType<Items.Armor.CowboySet.CowboyLegs>(), 1),
			(ModContent.ItemType<Items.Weapon.Thrown.TargetBottle>(), 25),
			(ItemID.SilverCoin, 90)
		};


		private SlayerQuestUGDesert()
        {
            _tasks.AddTask(new SlayTask(new int[] { NPCID.TombCrawlerHead, NPCID.FlyingAntlion, NPCID.WalkingAntlion, NPCID.GiantWalkingAntlion, NPCID.GiantFlyingAntlion}, 8));
        }
    }
}