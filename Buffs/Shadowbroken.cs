using Terraria;
using Terraria.ModLoader;
using SpiritMod.NPCs;
using Microsoft.Xna.Framework;

namespace SpiritMod.Buffs
{
	public class Shadowbroken : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shadowbroken");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.defense = (int)MathHelper.Max(npc.defense - 25, 0);
			npc.GetGlobalNPC<GNPC>().shadowbroken = true;
		}
	}
}