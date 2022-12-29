using SpiritMod.NPCs.Pagoda.SamuraiGhost;
using SpiritMod.NPCs.Pagoda.Yuurei;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SpiritMod.NPCs
{
	public class PagodaGlobalNPC : GlobalNPC
	{
		private readonly int[] validNPCs = new int[]
		{
			ModContent.NPCType<PagodaGhostPassive>(),
			ModContent.NPCType<PagodaGhostHostile>(),
			ModContent.NPCType<SamuraiPassive>(),
			ModContent.NPCType<SamuraiHostile>()
		};

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => validNPCs.Contains(entity.type);

		public override void OnSpawn(NPC npc, IEntitySource source) => MyWorld.pagodaSpawnTimer = 15;
	}
}