using SpiritMod.Tiles.Ambient;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Vanilla
{
	internal class IceRandomUpdate
	{
		public static void OnTick(int i, int j, int type)
		{
			bool belowSurfaceLayer = j > (int)Main.worldSurface && j < Main.maxTilesY - 250;

			if (!belowSurfaceLayer)
				return;

			if (WorldGen.genRand.NextBool(25) && !Framing.GetTileSafely(i, j).BottomSlope)
			{
				List<int> types = new List<int>(); //Pick a valid length based on the amount of space available

				if (EmptyTilesBelow(i, j) >= 3)
					types.Add(ModContent.TileType<UnstableIcicle2>());
				if (EmptyTilesBelow(i, j) >= 2)
					types.Add(ModContent.TileType<UnstableIcicle1>());
				if (EmptyTilesBelow(i, j) >= 1)
					types.Add(ModContent.TileType<UnstableIcicle>());
				else
					return;

				int style = type switch
				{
					TileID.CorruptIce => 1,
					TileID.FleshIce => 2,
					TileID.HallowedIce => 3,
					_ => 0
				};

				ushort placeType = (ushort)types[Main.rand.Next(types.Count)];
				if (WorldGen.PlaceObject(i, j + 1, placeType, false, style) && Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendObjectPlacement(-1, i, j + 1, placeType, style, 0, -1, -1);
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
