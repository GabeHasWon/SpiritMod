using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria.WorldBuilding;
using Terraria.Localization;

namespace SpiritMod.World.Micropasses;

internal class BoulderMicropass : Micropass
{
	public override string WorldGenName => "Boulder Micropass";

	static int MossType = -1;
	static Vector2 EndPoint = Vector2.Zero;

	static WeightedRandom<int> StoneType
	{
		get
		{
			WeightedRandom<int> types = new WeightedRandom<int>(WorldGen.genRand);
			types.Add(TileID.Stone, 1f);
			types.Add(TileID.BlueMoss, 0);
			types.Add(TileID.BrownMoss, 0);
			types.Add(TileID.RedMoss, 0);
			types.Add(TileID.PurpleMoss, 0);
			types.Add(TileID.GreenMoss, 0);
			return types;
		}
	}

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex) => passes.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
		progress.Message = Language.GetTextValue("Mods.SpiritMod.WorldGen.Boulders");

		float worldSize = Main.maxTilesX / 4200f;
		for (int i = 0; i < 6 * worldSize; i++)
		{
			int x = WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(100, Main.maxTilesX / 3) : WorldGen.genRand.Next((int)(Main.maxTilesX / 1.5f), Main.maxTilesX - 100);
			int y = (int)(Main.worldSurface * 0.35f);
			bool fail = false;
			int[] types = WorldGen.remixWorldGen ? [TileID.CorruptGrass, TileID.CrimsonGrass] : [TileID.Grass];

			while (!WorldGen.SolidTile(x, y) || types.Contains(Main.tile[x, y].TileType))
			{
				y++;

				if (y > Main.worldSurface)
				{
					fail = true;
					break;
				}
			}

			if (fail || Main.tileDungeon[Main.tile[x, y + 1].TileType])
			{
				i--;
				continue;
			}

			PlaceSingleBoulderFlatFaced(x, y);
		}
	}

	internal static void PlaceSingleBoulderFlatFaced(int x, int y)
	{
		Vector2 origin = new Vector2(x, y);
		List<float> sections = new();
		int sectCount = WorldGen.genRand.Next(5, 10);

		for (int i = 1; i < sectCount; i++)
		{
			float offset = WorldGen.genRand.NextFloat(MathHelper.TwoPi / sectCount / -2, MathHelper.TwoPi / sectCount / 2);
			sections.Add(MathHelper.TwoPi / i + offset);
		}

		sections.Reverse(); //So we get the whole thing in the right order
		float currentSect = 0;
		float nextSect = sections.First();

		List<int> sectSizes = new List<int>();

		for (int i = 0; i < sections.Count; ++i)
		{
			if (i == 0)
				sectSizes.Add(WorldGen.genRand.Next(2, 5));
			else
			{
				int newSize = sectSizes[i - 1] + (WorldGen.genRand.NextBool(2) ? WorldGen.genRand.Next(1, 3) : -WorldGen.genRand.Next(1, 3));

				if (newSize < 3)
					newSize = 3;
				else if (newSize > 5)
					newSize = 5;

				sectSizes.Add(newSize);
			}
		}

		float curLength = sectSizes[0];
		float nextLength = sectSizes[1];

		MossType = -1;
		if (WorldGen.genRand.NextBool(3))
			MossType = Main.rand.Next(new int[] { TileID.BlueMoss, TileID.BrownMoss, TileID.PurpleMoss, TileID.RedMoss, TileID.GreenMoss, TileID.BrownMoss });

		for (float radians = 0; radians < MathHelper.TwoPi; radians += MathHelper.TwoPi * 0.01f)
		{
			float length = MathHelper.Lerp(curLength, nextLength, (radians - currentSect) / nextLength);

			EndPoint = origin + new Vector2(length, 0).RotatedBy(radians);
			Utils.PlotTileLine(origin.ToWorldCoordinates(), EndPoint.ToWorldCoordinates(), 2f, PlaceTilesInLine);
		}
	}

	private static bool PlaceTilesInLine(int x, int y)
	{
		ushort type = (ushort)StoneType;
		Tile tile = Main.tile[x, y];

		if (tile.HasTile && StoneType.elements.Any(x => x.Item1 == tile.TileType))
			return true;

		if (tile.HasTile)
			WorldGen.ReplaceTile(x, y, type, 0);
		else
			WorldGen.PlaceTile(x, y, type, true, true);

		tile.Slope = SlopeType.Solid;
		tile.IsHalfBlock = false;

		if (MossType != -1 && Vector2.DistanceSquared(new Vector2(x, y), EndPoint) < 1.5f * 1.5f && tile.TileType == TileID.Stone && WorldMethods.AdjacentOpening(x, y))
			tile.TileType = (ushort)MossType;

		Tile.SmoothSlope(x, y);
		return true;
	}
}
