using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Tiles;

internal class SpreadHelper
{
	public static bool Spread(int i, int j, int type, int chance, params int[] validAdjacentTypes)
	{
		if (Main.rand.NextBool(chance))
		{
			var adjacents = OpenAdjacents(i, j, true, validAdjacentTypes);

			if (adjacents.Count == 0)
				return false;

			Point p = adjacents[Main.rand.Next(adjacents.Count)];

			Framing.GetTileSafely(p.X, p.Y).TileType = (ushort)type;
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendTileSquare(-1, p.X, p.Y, 1, TileChangeType.None);
			return true;
		}
		return false;
	}

	public static List<Point> OpenAdjacents(int i, int j, bool requiresAir, params int[] types)
	{
		var p = new List<Point>();
		for (int k = -1; k < 2; ++k)
			for (int l = -1; l < 2; ++l)
				if (!(l == 0 && k == 0) && Framing.GetTileSafely(i + k, j + l).HasTile && types.Contains(Framing.GetTileSafely(i + k, j + l).TileType))
					if (!requiresAir || OpenToAir(i + k, j + l))
						p.Add(new Point(i + k, j + l));

		return p;
	}

	public static bool OpenToAir(int i, int j)
	{
		for (int k = -1; k < 2; ++k)
			for (int l = -1; l < 2; ++l)
				if (!(l == 0 && k == 0) && !WorldGen.SolidOrSlopedTile(i + k, j + l))
					return true;

		return false;
	}
}
