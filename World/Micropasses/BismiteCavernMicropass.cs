using Microsoft.Xna.Framework;
using SpiritMod.Items.Sets.BismiteSet;
using SpiritMod.Tiles.Walls.Natural;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace SpiritMod.World.Micropasses;

internal class BismiteCavernMicropass : Micropass
{
	private static float Size => Main.maxTilesX / 4200f;

	public override string WorldGenName => "Bismite Caverns";

	public override int GetWorldGenIndexInsert(List<GenPass> tasks, ref bool afterIndex) => tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

	public override void Run(GenerationProgress progress, GameConfiguration config)
	{
		List<Point16> points = new();
		float repeats = Size * 24;
		int tries = 0;

		for (int i = 0; i < repeats; ++i)
		{
			if (++tries > 2000) //Short circuit if needed
				return;

			int x = WorldGen.genRand.Next(Main.maxTilesX / 6, Main.maxTilesX / 6 * 5);
			int y = WorldGen.genRand.Next((int)Main.worldSurface + 50, Main.maxTilesY - 400);

			if (!Collision.SolidCollision(new Vector2(x - 2, y - 2) * 16, 16 * 4, 16 * 4)) //Get at least a moderately sized opening
				points.Add(new(x, y));
			else
				i--;
		}

		foreach (var item in points)
			PopulateCavern(item);
	}

	private static void PopulateCavern(Point16 item)
	{
		int distance = (int)(Size * 12);

		for (int i = -distance + item.X; i < distance + item.X; ++i)
		{
			for (int j = -distance + item.Y; j < distance + item.Y; ++j)
			{
				if (Vector2.DistanceSquared(item.ToVector2(), new Vector2(i, j)) < distance * distance)
				{
					Tile tile = Main.tile[i, j];

					if (WorldGen.SolidTile(i, j + 1) && !tile.HasTile && WorldGen.genRand.NextBool(3)) //Crystals
						WorldGen.PlaceTile(i, j, ModContent.TileType<BismiteCrystalTile>(), true);

					if (WorldGen.genRand.NextBool(140) && tile.TileType == TileID.Stone)
						WorldGen.TileRunner(i, j, WorldGen.genRand.Next(3, 11), 10, ModContent.TileType<BismiteCrystalOre>(), false, 0, 0, false, true, TileID.Dirt);
				}
			}
		}

		for (int i = 0; i < Size * 3; ++i)
			SpawnColumns(item);
	}

	private static void SpawnColumns(Point16 item)
	{
		static Vector2 Offset() => new(WorldGen.genRand.Next(-6, 6), WorldGen.genRand.Next(-6, 6));

		Vector2 direction = new Vector2(1, 0).RotatedByRandom(MathHelper.TwoPi);
		Vector2 start = item.ToVector2() + Offset() + new Vector2(0.5f);

		while (!WorldGen.SolidTile((int)start.X, (int)start.Y))
			start -= direction;

		Vector2 end = item.ToVector2() + Offset() + new Vector2(0.5f);

		while (!WorldGen.SolidTile((int)end.X, (int)end.Y))
			end += direction;

		int repeats = (int)start.Distance(end);

		for (int j = 0; j < repeats; ++j)
		{
			int left = WorldGen.genRand.Next(-2, 0);
			int right = WorldGen.genRand.Next(1, 3);

			for (int k = left; k < right; ++k)
			{
				Vector2 position = start + (direction * j);
				position += direction.RotatedBy(MathHelper.PiOver2) * k;
				Point16 p = position.ToPoint16();

				WorldGen.PlaceWall(p.X, p.Y, ModContent.WallType<BismiteWall>(), true);
			}
		}
	}
}
