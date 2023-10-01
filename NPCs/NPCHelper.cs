using SpiritMod.World;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs
{
	public static class NPCHelper
	{
		public static void BuffImmune(int type, bool whipsToo = false)
		{
			if (whipsToo)
				NPCID.Sets.ImmuneToAllBuffs[type] = true;
			else
				NPCID.Sets.ImmuneToRegularBuffs[type] = true;
		}

		public static void BuffImmune(ModNPC npc, bool whipsToo = false) => BuffImmune(npc.Type, whipsToo);

		public static void ImmuneTo(ModNPC npc, params int[] buffs)
		{
			foreach (int buff in buffs)
				NPCID.Sets.SpecificDebuffImmunity[npc.Type][buff] = true;
		}

		public static void ImmuneTo<T>(ModNPC npc, params int[] buffs) where T : ModBuff => ImmuneTo(npc, buffs.With(ModContent.BuffType<T>()));
		public static void ImmuneTo<T1, T2>(ModNPC npc, params int[] buffs) where T1 : ModBuff where T2 : ModBuff => ImmuneTo(npc, buffs.With(new int[] { ModContent.BuffType<T1>(), ModContent.BuffType<T2>() }));

		public static void ImmuneTo<T1, T2, T3>(ModNPC npc, params int[] buffs) where T1 : ModBuff where T2 : ModBuff where T3 : ModBuff 
			=> ImmuneTo(npc, buffs.With(new int[] { ModContent.BuffType<T1>(), ModContent.BuffType<T2>(), ModContent.BuffType<T3>() }));
	}
}
