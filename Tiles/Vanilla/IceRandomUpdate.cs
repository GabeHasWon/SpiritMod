using SpiritMod.Tiles.Ambient;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Vanilla
{
	internal class IceRandomUpdate
	{
		public static void OnTick(int i, int j)
		{
			bool belowSurfaceLayer = j > (int)Main.worldSurface && j < Main.maxTilesY - 250;

			if (!belowSurfaceLayer)
				return;

			if (WorldGen.genRand.NextBool(20))
			{
				if (!Framing.GetTileSafely(i, j).BottomSlope)
				{
					List<int> types = new List<int>(); //Pick a valid length based on the amount of space available
					if (EmptyTilesBelow(i, j) >= 1)
						types.Add(ModContent.TileType<UnstableIcicle1>());
					if (EmptyTilesBelow(i, j) >= 2)
						types.Add(ModContent.TileType<UnstableIcicle1>());
					if (EmptyTilesBelow(i, j) >= 3)
						types.Add(ModContent.TileType<UnstableIcicle2>());

					if (!types.Any())
						return;

					ushort type = (ushort)types[Main.rand.Next(types.Count)];
					WorldGen.PlaceObject(i, j + 1, type);
					if (Main.netMode != NetmodeID.SinglePlayer)
						NetMessage.SendObjectPlacment(-1, i, j + 1, type, 0, 0, -1, -1);
				}
			}

			static int EmptyTilesBelow(int i, int j)
			{
				for (int t = 0; t < 3; t++)
				{
					Tile tile = Framing.GetTileSafely(i, j + t + 1);

					if (tile.HasTile || tile.LiquidType == LiquidID.Lava)
						return t;
				}
				return 3;
			}
		}
	}
}
