using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace SpiritMod.Buffs;

public class WitheringLeaf : ModBuff
{
	public override void Update(NPC npc, ref int buffIndex)
	{
		if (Main.rand.NextBool(6))
			Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.GrassBlades);
	}
}