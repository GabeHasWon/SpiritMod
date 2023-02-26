using Terraria;
using Terraria.DataStructures;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnZombies : ChanceEffect
	{
		public override bool Unlucky => true;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			int a = NPC.NewNPC(source, (tileCoords.X * 16) + -8 - 16 - 16, (tileCoords.Y * 16) + 6, 21);
			int b = NPC.NewNPC(source, (tileCoords.X * 16) + -8 - 16, (tileCoords.Y * 16) + 6, 21);
			int c = NPC.NewNPC(source, (tileCoords.X * 16) + 8, (tileCoords.Y * 16) + 6, 21);
			int d = NPC.NewNPC(source, (tileCoords.X * 16) + 24 + 16, (tileCoords.Y * 16) + 6, 21);
			int e = NPC.NewNPC(source, (tileCoords.X * 16) + 24 + 32, (tileCoords.Y * 16) + 6, 21);
			
			Main.npc[a].netUpdate = true;
			Main.npc[b].netUpdate = true;
			Main.npc[c].netUpdate = true;
			Main.npc[d].netUpdate = true;
			Main.npc[e].netUpdate = true;
		}
	}
}