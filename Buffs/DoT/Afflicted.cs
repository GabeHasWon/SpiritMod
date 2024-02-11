using SpiritMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.DoT
{
	public class Afflicted : ModBuff
	{
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<GNPC>().afflicted = true;
			npc.velocity *= .95f;

			if (Main.rand.NextBool(4))
			{
				Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.BubbleBlock, Scale: 3).noGravity = true;
				Dust.NewDust(npc.position, npc.width, npc.height, DustID.UnusedWhiteBluePurple, Scale: 3);
			}
		}
	}
}