using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem.Quests
{
    public class SlayerQuestScreechOwls : Quest
    {
		public override int QuestClientID => NPCID.Guide;
		public override int Difficulty => 2;
		public override string QuestCategory => "Slayer";

		public override (int, int)[] QuestRewards => _rewards;
		private readonly (int, int)[] _rewards = new[]
		{
			(ModContent.ItemType<Items.Armor.Masks.WinterHat>(), 1),
			(ItemID.MusicBox, 1),
			(ModContent.ItemType<Items.Sets.FrigidSet.FrigidFragment>(), 5),
			(ModContent.ItemType<Items.Weapon.Thrown.TargetBottle>(), 25),
			(ItemID.Snowball, 50),
			(ItemID.SilverCoin, 25)
		};

		private SlayerQuestScreechOwls()
        {
            _tasks.AddTask(new SlayTask(ModContent.NPCType<NPCs.ScreechOwl.ScreechOwl>(), 2, null, new QuestPoolData(0.4f, true)));
        }

    	public override void OnQuestComplete()
		{
            bool showUnlocks = true;
            QuestManager.UnlockQuest<SlayerQuestValkyrie>(showUnlocks);
			QuestManager.UnlockQuest<SlayerQuestDrBones>(showUnlocks);
			QuestManager.UnlockQuest<SlayerQuestNymph>(showUnlocks);
			QuestManager.UnlockQuest<SlayerQuestUGDesert>(showUnlocks);
			QuestManager.UnlockQuest<SlayerQuestCavern>(showUnlocks);

			base.OnQuestComplete();
        }
    }
}