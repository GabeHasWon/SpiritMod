using Terraria;
using Terraria.DataStructures;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnZombies : ChanceEffect
	{
		public override bool Unlucky => true;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			int loops = 5;
			for (int i = 0; i < loops; i++)
			{
				int increment = 16;
				int offset = -(increment * (loops / 5)) + (i * increment);

				NPC npc = NPC.NewNPCDirect(source, (tileCoords.X * 16) + offset, (tileCoords.Y * 16) + 6, 21);
				npc.netUpdate = true;
			}
		}
	}
}