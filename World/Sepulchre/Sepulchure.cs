using Microsoft.Xna.Framework;
using SpiritMod.Utilities.Noise;
using SpiritMod.World.Sepulchre;
using SpiritMod.Tiles.Ambient;
using SpiritMod.Tiles.Block;
using SpiritMod.Tiles.Walls.Natural;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.Localization;

namespace SpiritMod.World.Sepulchre;

public class SepulchureSystem : ModSystem
{
	private static int Wall => ModContent.WallType<SepulchreWallTile>();
	private static int Tile => ModContent.TileType<SepulchreBrick>();
	private static int TileTwo => ModContent.TileType<SepulchreBrickTwo>();

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
	{
		int index = tasks.FindIndex(genpass => genpass.Name.Equals("Cave Walls"));

		if (index == -1 || ModLoader.HasMod("Remnants"))
			return;

		tasks.Insert(++index, new PassLegacy("Sepulchure", (GenerationProgress progress, GameConfiguration config) =>
		{
			progress.Message = Language.GetTextValue("Mods.SpiritMod.WorldGen.Sepulchre");

			float old = Main.soundVolume;
			Main.soundVolume = 0;

			for (int i = 0; i < Main.maxTilesX / 250; i++)
			{
				var position = new Vector2(WorldGen.genRand.NextFloat(Main.maxTilesX * 0.2f, Main.maxTilesX * 0.8f), 
					WorldGen.genRand.Next(Main.maxTilesY - 500, Main.maxTilesY - 300));
				int repeats = 0;

				while (GenVars.structures.CanPlace(new Rectangle((int)position.X - 15, (int)position.Y - 25, 30, 50)))
				{
					repeats++;
					position = new Vector2(WorldGen.genRand.NextFloat(Main.maxTilesX * 0.2f, Main.maxTilesX * 0.8f),
					WorldGen.genRand.Next(Main.maxTilesY - 500, Main.maxTilesY - 300));

					if (repeats > 2000)
						break;
				}

				if (repeats > 2000)
				{
					continue;
				}

				CreateSepulchre(position);
				progress.Value = i / (Main.maxTilesX / 250f);
				GenVars.structures.AddProtectedStructure(new Rectangle((int)position.X - 15, (int)position.Y - 25, 30, 50), 10);
			}

			Main.soundVolume = old;
		}, 300f));
	}

	public void CreateSepulchre(Vector2 position)
	{
		int[] invalidTypes = new int[]
		{
			TileID.BeeHive,
			TileID.BlueDungeonBrick,
			TileID.GreenDungeonBrick,
			TileID.PinkDungeonBrick,
			TileID.LihzahrdBrick,
			TileTwo
		};

		const int CheckDistanceX = 75;
		const int CheckDistanceY = 125;

		bool fail = false;
		for (int x = (int)position.X - CheckDistanceX; x < (int)position.X + CheckDistanceX; x++)
		{
			for (int y = (int)position.Y - CheckDistanceY; y < (int)position.Y + CheckDistanceY; y++)
			{
				for (int z = 0; z < invalidTypes.Length; z++)
				{
					int type = Framing.GetTileSafely(x, y).TileType;
					if (type == invalidTypes[z] || !TileID.Sets.CanBeClearedDuringGeneration[type])
					{
						fail = true;
						break;
					}
				}

				if (fail)
					break;
			}
		}

		int tries = 0;

		while (fail)
		{
			position = new Vector2(WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2f), (int)(Main.maxTilesX * 0.8f)), WorldGen.genRand.Next(Main.maxTilesY - 500, Main.maxTilesY - 300));
			fail = false;
			for (int x = (int)position.X - CheckDistanceX; x < (int)position.X + CheckDistanceX; x++) //Main structure, DO NOT USE LOOPTHROUGHTILES HERE
			{
				for (int y = (int)position.Y - CheckDistanceY; y < (int)position.Y + CheckDistanceY; y++)
				{
					for (int z = 0; z < invalidTypes.Length; z++)
						if (Framing.GetTileSafely(x, y).TileType == invalidTypes[z])
						{
							fail = true;
							break;
						}

					if (fail)
						break;
				}
			}

			if (++tries > 100)
				return;
		}

		var noiseType = new PerlinNoiseTwo(WorldGen._genRandSeed);
		int i = (int)position.X;
		int j = (int)position.Y;

		for (int x = i - 25; x < i + 25; x++) //Main structure, DO NOT USE LOOPTHROUGHTILES HERE
		{
			for (int y = j - 20; y < j + 20; y++)
			{
				if (noiseType.Noise2D((x * 600) / (float)4200, (y * 600) / (float)1200) > 0.85f) //regular rooms
					CreateRoom(x, y, WorldGen.genRand.Next(5, 10), WorldGen.genRand.Next(5, 10));

				if (noiseType.Noise2D((x * 600) / (float)4200, (y * 600) / (float)1200) > 0.95f || (x == i && y == j)) //towers
				{
					int towerheight = 30;
					int width = WorldGen.genRand.Next(5, 8);
					CreateRoom(x, y - towerheight, width, towerheight - 8);
					CreateHalfCircle(x + (width / 2) + 2, y - (int)(towerheight * 1.5f), width + 4);
				}
			}
		}

		WallCleanup(i, j);

		for (int x = i - 50; x < i + 50; x++)
			for (int y = j - 90; y < j + 50; y++)
				if (Main.rand.NextBool(25) && (Main.tile[x, y].TileType == Tile || Main.tile[x, y].TileType == TileTwo) && Main.tile[x, y].HasTile)
					WorldGen.KillTile(x, y, false, false, true);

		CreateChests(i, j);
		PolishSepulchre(i, j);
		PlaceEnemies(i, j);
		RemoveWaterFromRegion(50, 40, position - new Vector2(25, 20));
	}

	public static void RemoveWaterFromRegion(int width, int height, Vector2 startingPoint)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Tile tile = Main.tile[i + (int)startingPoint.X, j + (int)startingPoint.Y];

				if (tile.LiquidType == 0 && tile.LiquidAmount > 64)
					tile.LiquidAmount = 0;
			}
		}
	}

	public delegate void AtTile(int x, int y);

	public void WallCleanup(int i, int j)
	{
		var delegateList = new List<AtTile>
		{
			delegate (int x, int y) //clear away 2 wide walls
			{
				DeleteWallVertical(x, y, 6);
				DeleteWallHorizontal(x, y, 6);
			},
			delegate (int x, int y) //kill orphan tiles
			{
				DeleteOrphan(x, y);
			},
			delegate (int x, int y) //clear away 2 wide walls
			{
				DeleteWallVertical(x, y, 4);
				DeleteWallHorizontal(x, y, 4);
			}
		};
		foreach (AtTile atTile in delegateList)
			LoopThroughTiles(i, j, atTile);
	}

	public void CreateChests(int i, int j)
	{
		for (int x = i - 50; x < i + 50; x++)
		{
			for (int y = j - 90; y < j + 50; y++)
			{
				Tile me = Main.tile[x, y];
				Tile below = Main.tile[x, y + 1];

				if ((below.TileType == Tile || below.TileType == TileTwo) && Main.rand.NextBool(100) && me.WallType == Wall && !WorldGen.SolidOrSlopedTile(x, y))
				{
					WorldGen.PlaceTile(x + 1, y + 1, below.TileType);
					WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<SepulchreChestTile>(), false, 0);

					if (Main.tile[x, y - 1].TileType == (ushort)ModContent.TileType<SepulchreChestTile>())
						goto skipLoops;
				}
			}
		}

	skipLoops:
		return;
	}

	public void PlaceEnemies(int i, int j)
	{
		bool success = false;
		bool draugrSuccess = false;
		int tries = 0;

		while (!success || !draugrSuccess)
		{
			for (int x = i - 50; x < i + 50; x++)
			{
				for (int y = j - 90; y < j + 50; y++)
				{
					if (Main.tile[x, y + 1].TileType == TileID.Platforms && !success && Main.rand.NextBool(100) && Main.tile[x, y].WallType == Wall)
					{
						WorldGen.KillTile(x, y);
						WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<ScreamingTomeTile>(), true);

						if (Main.tile[x, y].TileType == (ushort)ModContent.TileType<ScreamingTomeTile>())
							success = true;
					}

					if (!draugrSuccess && (Main.tile[x, y + 1].TileType == Tile || Main.tile[x, y + 1].TileType == TileTwo) && Main.rand.NextBool(25) && Main.tile[x, y].WallType == Wall)
					{
						WorldGen.KillTile(x, y);
						WorldGen.PlaceObject(x, y, ModContent.TileType<CursedArmor>(), true);

						if (Main.tile[x, y].TileType == (ushort)ModContent.TileType<CursedArmor>())
							draugrSuccess = true;
					}
				}
			}

			if (++tries > 6000)
				break;
		}
	}

	public void PolishSepulchre(int i, int j)
	{
		var noiseType2 = new PerlinNoiseTwo(WorldGen._genRandSeed);

		var delegateList = new List<AtTile>
		{
			delegate (int x, int y) //platforms
			{
				if (Main.rand.NextBool(50) && Main.tile[x - 1, y].TileType == Tile && Main.tile[x - 1, y].HasTile)
					CreateShelf(x, y, 50, 12, false);
			},
			delegate (int x, int y) //cursed armor
			{
				if ((Main.tile[x, y + 1].TileType == Tile || Main.tile[x, y + 1].TileType == TileTwo) && Main.rand.NextBool(22) && Main.tile[x, y].WallType == Wall)
					WorldGen.PlaceObject(x, y, ModContent.TileType<CursedArmor>(), true);
			},
			delegate (int x, int y) //pots
			{
				if ((Main.tile[x, y + 1].TileType == Tile || Main.tile[x, y + 1].TileType == TileTwo) && Main.rand.NextBool(4) && Main.tile[x, y].WallType == Wall)
				{
					int potType = 0;
					potType = Main.rand.Next(2) switch
					{
						0 => ModContent.TileType<SepulchrePot1>(),
						_ => ModContent.TileType<SepulchrePot2>(),
					};
					WorldGen.PlaceObject(x, y, potType, true);
				}
			},
			delegate (int x, int y) //cracks in walls
			{
				if (noiseType2.Noise2D((x * 200) / (float)1200, (y * 200) / (float)1200) > 0.75f && Main.tile[x, y].WallType == Wall)
					Main.tile[x, y].WallType = 0;
			},
			delegate (int x, int y) //chandeliers
			{
				if ((Main.tile[x, y - 1].TileType == Tile || Main.tile[x, y - 1].TileType == TileTwo) && Main.rand.NextBool(12) && Main.tile[x, y].WallType == Wall)
					WorldGen.PlaceObject(x, y, ModContent.TileType<SepulchreChandelier>(), true);
			},
			delegate (int x, int y) //Windows
			{
				if (WorldGen.genRand.NextBool(30)&& (Main.tile[x - 1, y].TileType == Tile || Main.tile[x - 1, y].TileType == TileTwo) && Main.tile[x - 1, y].HasTile)
				{
					CreateWindowRow(x, y, 50, ModContent.TileType<SepulchreWindowOne>());
				}
			},
			delegate (int x, int y) //shelves
			{
				if (Main.rand.NextBool(20) && (Main.tile[x - 1, y].TileType == Tile || Main.tile[x - 1, y].TileType == TileTwo) && Main.tile[x - 1, y].HasTile)
					CreateShelf(x, y, Main.rand.Next(4, 8), 12, true);

				if (Main.rand.NextBool(20) && (Main.tile[x - 1, y].TileType == Tile || Main.tile[x - 1, y].TileType == TileTwo) && Main.tile[x + 1, y].HasTile)
					CreateShelfBackwards(x, y, Main.rand.Next(4, 8), 12, true);
			},
			delegate (int x, int y) //cobwebs
			{
				if (noiseType2.Noise2D((x * 100) / (float)1200, (y * 100) / (float)1200) > 0.70f && Main.tile[x, y].WallType == Wall && !Main.tile[x, y].HasTile)
					WorldGen.PlaceTile(x, y, TileID.Cobweb, true);
			},
			delegate (int x, int y) //torches
			{
				if (Main.rand.NextBool(200) && Main.tile[x, y].WallType == Wall && !Main.tile[x, y].HasTile)
					WorldGen.PlaceTile(x, y, 4, true, false, -1, 8);
			},
			delegate (int x, int y) //mirrors
			{
				if (Main.rand.NextBool(200) && Main.tile[x, y].WallType == Wall && !Main.tile[x, y].HasTile && !MirrorsNearby(x, y))
					WorldGen.PlaceObject(x, y, ModContent.TileType<SepulchreMirror>(), true);
			}
		};

		foreach (AtTile atTile in delegateList)
			LoopThroughTiles(i, j, atTile);
	}

	public static void LoopThroughTiles(int i, int j, AtTile atTile)
	{
		for (int x = i - 50; x < i + 50; x++)
			for (int y = j - 90; y < j + 50; y++)
				atTile.Invoke(x, y);
	}

	public void CreateHalfCircle(int x, int y, int diameter)
	{
		for (float i = -3.14f; i < 0f; i += 0.01f)
		{
			for (float j = 0; j <= diameter; j += 0.5f)
			{
				Vector2 pos = i.ToRotationVector2();
				pos *= j;
				pos.Y += Math.Abs(pos.X);

				Tile tile = Main.tile[(int)pos.X + x, (int)pos.Y + y];

				if (Math.Abs(j - diameter) <= 0.5f)
				{
					WorldGen.KillTile((int)pos.X + x, (int)pos.Y + y);
					WorldGen.PlaceTile((int)pos.X + x, (int)pos.Y + y, Tile, true);
				}
				else
				{
					if (tile.TileType != Tile && tile.TileType != TileTwo)
						ClearTile((int)pos.X + x, (int)pos.Y + y);

					WorldGen.PlaceWall((int)pos.X + x, (int)pos.Y + y, Wall, true);
				}
			}
		}
	}

	public static bool MirrorsNearby(int x, int y)
	{
		for (int i = x - 6; i < x + 6; i++)
		{
			for (int j = y - 6; j < y + 6; j++)
			{
				if (Main.tile[i, j].TileType == ModContent.TileType<SepulchreMirror>() || Main.tile[i, j].TileType == ModContent.TileType<SepulchreWindowOne>())
					return true;
			}
		}

		return false;
	}

	public static void PlaceShelfItem(int x, int y)
	{
		switch (Main.rand.Next(3))
		{
			case 0:
				WorldGen.PlaceTile(x, y, 50, true, false, -1, Main.rand.Next(5));
				break;
			case 1:
				WorldGen.PlaceTile(x, y, 13, true, false, -1, Main.rand.Next(3));
				break;
		}
	}

	public void CreateShelf(int i, int j, int length, int platformType, bool placeobjects)
	{
		for (int x = i; x < i + length; x++)
		{
			if (Main.tile[x, j].HasTile || Main.tile[x, j - 1].HasTile || Main.tile[x, j - 2].HasTile || Main.tile[x, j].WallType != Wall)
				return;

			WorldGen.PlaceTile(x, j, 19, true, false, -1, platformType);

			if (placeobjects)
				PlaceShelfItem(x, j - 1);
		}
	}

	public void CreateShelfBackwards(int i, int j, int length, int platformType, bool placeobjects)
	{
		for (int x = i; x > i - length; x--)
		{
			if (Main.tile[x, j].HasTile || Main.tile[x, j - 1].HasTile || Main.tile[x, j - 2].HasTile || Main.tile[x, j].WallType != Wall)
				return;

			WorldGen.PlaceTile(x, j, 19, true, false, -1, platformType);

			if (placeobjects)
				PlaceShelfItem(x, j - 1);
		}
	}

	public void CreateWindowRow(int i, int j, int length, int windowType)
	{
		for (int x = i + 2; x < i + length; x += 5)
		{
			if (Main.tile[x, j].HasTile || Main.tile[x, j - 1].HasTile || Main.tile[x, j - 2].HasTile || Main.tile[x, j].WallType != Wall)
				return;

			if (!Main.rand.NextBool(3))
				WorldGen.PlaceObject(x, j, windowType, true);
		}
	}

	public void CreateRoom(int i, int j, int width, int height)
	{
		for (int x = i - (width / 2); x <= i + (width * 2); x++)
		{
			for (int y = j - (height / 2); y <= j + (height * 2); y++)
			{
				Tile tile = Framing.GetTileSafely(x, y);

				if (y == j - (height / 2) || y == j + (height * 2) || x == i - (width / 2) || x == i + (width * 2))
				{
					tile.ClearTile();
					WorldGen.PlaceTile(x, y, TileTwo, true);
				}
				else if (y == j - (height / 2) + 1 || y == j + (height * 2) - 1 || x == i - (width / 2) + 1 || x == i + (width * 2) - 1)
				{
					tile.ClearTile();
					WorldGen.PlaceTile(x, y, TileTwo, true);
				}
				else
				{
					tile.ClearEverything();
					WorldGen.PlaceWall(x, y, Wall, true);
				}
			}
		}
	}

	public void DeleteWallVertical(int i, int j, int width)
	{
		width /= 2;

		if (!Main.tile[i, j - width].HasTile && !Main.tile[i, j + width].HasTile && Main.tile[i, j + width].WallType == Wall && Main.tile[i, j - width].WallType == Wall)
		{
			for (int y = j - width; y <= j + width; y++)
			{
				ClearTile(i, y);
				WorldGen.PlaceWall(i, y, Wall, true);
			}
		}
	}

	public void DeleteWallHorizontal(int i, int j, int width)
	{
		width /= 2;

		if (!Main.tile[i - width, j].HasTile && !Main.tile[i + width, j].HasTile && Main.tile[i + width, j].WallType == Wall && Main.tile[i - width, j].WallType == Wall)
		{
			for (int x = i - width; x <= i + width; x++)
			{
				ClearTile(x, j);
				WorldGen.PlaceWall(x, j, Wall, true);
			}
		}
	}

	public void DeleteOrphan(int i, int j)
	{
		if (!Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile && !Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile &&
			(Main.tile[i, j].TileType == Tile || Main.tile[i, j].TileType == TileTwo))
			WorldGen.KillTile(i, j, false, false, true);
	}

	private static void ClearTile(int i, int j)
	{
		WorldGen.KillTile(i, j, false, false, true);
		WorldGen.KillWall(i, j);
	}
}