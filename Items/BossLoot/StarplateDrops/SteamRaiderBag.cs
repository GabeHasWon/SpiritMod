using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.DonatorVanity;
using SpiritMod.NPCs;

namespace SpiritMod.Items.BossLoot.StarplateDrops
{
	public class SteamRaiderBag : BossBagItem
	{
		internal override string BossName => "Starplate Voyager";

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			itemLoot.AddCommon<StarMap>();
			itemLoot.AddCommon<CosmiliteShard>(1, 6, 10);
			AddBossItems<StarplateMask, Trophy3>(itemLoot, 4..6);
		}
	}
}
