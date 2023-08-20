using SpiritMod.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.DoT
{
	public class DoomDestiny : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star Cut");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Terraria.ID.BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) => player.GetSpiritPlayer().doomDestiny = true;

		public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<GNPC>().doomDestiny = true;
	}
}
