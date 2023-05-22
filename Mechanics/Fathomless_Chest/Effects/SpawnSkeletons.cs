using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnSkeletons : ChanceEffect
	{
		public override byte WhoAmI => 11;

		public override bool Unlucky => true;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int loops = 5;
				for (int i = 0; i < loops; i++)
				{
					int increment = 16;
					int offset = -(increment * (loops / 5)) + (i * increment);

					int id = NPC.NewNPC(source, (tileCoords.X * 16) + offset, (tileCoords.Y * 16) + 6, 21);
					Main.npc[id].netUpdate2 = true;
				}
			}
		}
	}
}