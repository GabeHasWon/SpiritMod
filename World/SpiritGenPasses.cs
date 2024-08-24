﻿using Microsoft.Xna.Framework;
using SpiritMod.Items;
using SpiritMod.Items.BossLoot.StarplateDrops;
using SpiritMod.Items.Sets.FloranSet;
using SpiritMod.Items.Sets.GraniteSet;
using SpiritMod.Items.Sets.MarbleSet;
using SpiritMod.Tiles.Ambient;
using SpiritMod.Tiles.Ambient.IceSculpture;
using SpiritMod.Tiles.Ambient.IceSculpture.Hostile;
using SpiritMod.Tiles.Ambient.SpaceCrystals;
using SpiritMod.Tiles.Ambient.SurfaceIce;
using SpiritMod.Tiles.Ambient.Underground;
using SpiritMod.Tiles.Block;
using SpiritMod.Tiles.Furniture;
using SpiritMod.Tiles.Furniture.Paintings;
using SpiritMod.Tiles.Piles;
using SpiritMod.Tiles.Walls.Natural;
using SpiritMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace SpiritMod.World
{
	public static class SpiritGenPasses
	{
		// Please organize fields with the proper comment, i.e.
		// type field = value; //Allows y to do x

		private static List<DecorSpamData> decorSpam = new List<DecorSpamData>(); //For ambient decor spam genpass merging

		// Please put all your methods in regions
		// Genpass methods should have their region name be "GENPASS: CAPITALIZED GENPASS NAME"
		// Non-genpass utility methods that are used by genpasses can be named anything else

		#region GENPASS: SPIRIT MICROS
		public static void MicrosPass(GenerationProgress progress, GameConfiguration config)
		{
			progress.Message = Language.GetTextValue("Mods.SpiritMod.WorldGen.Hideouts");

			if (ModContent.GetInstance<SpiritClientConfig>().DoubleHideoutGeneration)
			{
				new BanditHideout().Generate();
				GoblinTower.Generate();
				MyWorld.gennedBandits = true;
				MyWorld.gennedTower = true;
			}
			else
			{
				if (WorldGen.genRand.NextBool(2))
				{
					new BanditHideout().Generate();
					MyWorld.gennedBandits = true;
				}
				else
				{
					GoblinTower.Generate();
					MyWorld.gennedTower = true;
				}
			}

			progress.Message = Language.GetTextValue("Mods.SpiritMod.WorldGen.Caches");

			int siz = (int)((Main.maxTilesX / 4200f) * 7);
			int repeats = WorldGen.genRand.Next(siz, siz + 4);

			for (int k = 0; k < repeats - 2; k++)
				GenerateCrateStash();

			for (int k = 0; k < (repeats / 2 + 1); k++)
				GenerateCrateStashJungle();

			if (WorldGen.genRand.NextBool(2))
				for (int k = 0; k < (repeats / 4); k++)
					GenerateStoneDungeon();

			for (int k = 0; k < WorldGen.genRand.Next(5, 7); k++)
				GenerateGemStash();

			progress.Message = Language.GetTextValue("Mods.SpiritMod.WorldGen.Pagoda");

			GeneratePagoda();
			GenerateZiggurat();
		}

		private static readonly List<Point> houseLocations = new();

		internal static void StealIslandInfo(On_WorldGen.orig_IslandHouse orig, int i, int j, int islandStyle)
		{
			houseLocations.Add(new(i, j));
			orig(i, j, islandStyle);
		}

		public static void AvianIslandsPass(GenerationProgress progress, GameConfiguration config)
		{
			progress.Message = Language.GetTextValue("Mods.SpiritMod.WorldGen.AvianIslands");

			float repeats = Main.maxTilesX / 4200f * 2f;

			for (int i = 0; i < (int)repeats; i++)
				GenerateBoneIsland(); //2 islands in a small world
		}
		#endregion Spirit Micros

		private static void BlacklistBlocks(int maxTries, int width, int height, Rectangle randomizationRange, ref Point tileCheckPos, out bool failed)
		{
			int tries = 0;
			failed = false;

			int[] TileBlacklist = GlobalExtensions.TileSet<BriarGrass, FloranOreTile, BlastStone>().With(TileID.Ebonsand, TileID.Ebonstone, TileID.Crimstone, 
				TileID.Crimsand, TileID.LihzahrdBrick);
			int[] WallBlacklist = [ModContent.WallType<ReachWallNatural>(), ModContent.WallType<ReachStoneWall>()];

			do
			{
				tileCheckPos.X = WorldGen.genRand.Next(randomizationRange.X, randomizationRange.X + randomizationRange.Width);
				tileCheckPos.Y = WorldGen.genRand.Next(randomizationRange.Y, randomizationRange.Y + randomizationRange.Height);

				int xDist = width;
				int yDist = height;
				int xCenter = tileCheckPos.X;
				int yCenter = tileCheckPos.Y;

				bool blackListedTile = false;
				for (int i = -xDist / 2; i <= xDist / 2; i++)
				{
					for (int j = -yDist / 2; j <= yDist / 2; j++)
					{
						Tile tile = Framing.GetTileSafely(xCenter + i, yCenter + j);
						if (TileBlacklist.Contains(tile.TileType) || WallBlacklist.Contains(tile.WallType))
						{
							blackListedTile = true;
							break;
						}
					}

					if (blackListedTile)
						break;
				}

				if (blackListedTile) //Keep going until blacklisted tiles are not hit
				{
					tries++;
					if (tries >= maxTries) //If it tries too many times, break and cancel the generation, as to not softlock forever
					{
						failed = true;
						break;
					}

					continue;
				}

				if (!GenVars.structures.CanPlace(new Rectangle(xCenter - xDist / 2, yCenter - yDist / 2, xDist, yDist))) // Fail if on an existing structure
				{
					tries++;
					continue;
				}

				break;
			} while (true);
		}

		#region Campsite
		private static void GenerateCampsite()
		{
			int[,] CampShape1 = new int[,]
			{
				{6,6,6,0,0,0,0,0,0,0,6,6,6,6},
				{6,6,0,0,0,0,0,0,0,0,0,6,6,6},
				{6,0,0,0,0,0,0,0,0,0,0,0,6,6},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,3,0,0,0,0,2,0,0,0,4,0,5,0},
				{1,1,1,1,1,1,1,1,1,1,1,1,1,1},
			};

			while (true)
			{
				// Select a place in the first 6th of the world
				int fireX = Main.spawnTileX + WorldGen.genRand.Next(-800, 800); // from 50 since there's a unaccessible area at the world's borders
				
				if (WorldGen.genRand.NextBool()) // 50% of choosing the last 6th of the world
					fireX = Main.maxTilesX - fireX;

				int fireY = 0;
				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(fireX, fireY) && fireY <= Main.worldSurface)
					fireY++;

				// If we went under the world's surface, try again
				if (fireY > Main.worldSurface)
					continue;

				if (!GenVars.structures.CanPlace(new Rectangle(fireX, fireY + 1, CampShape1.GetLength(0), CampShape1.GetLength(1))))
					continue;

				Tile tile = Main.tile[fireX, fireY];

				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if (tile.TileType != TileID.Dirt && tile.TileType != TileID.Grass && tile.TileType != TileID.Stone)
					continue;

				// place the tower
				PlaceCampsite(fireX, fireY + 1, CampShape1);
				GenVars.structures.AddProtectedStructure(new Rectangle(fireX, fireY + 1, CampShape1.GetLength(0), CampShape1.GetLength(1)));
				break;
			}
		}

		private static void PlaceCampsite(int i, int j, int[,] BlocksArray)
		{
			for (int y = 0; y < BlocksArray.GetLength(0); y++)
			{
				for (int x = 0; x < BlocksArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;

					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);

						switch (BlocksArray[y, x])
						{
							case 0:
								tile.ClearTile();
								break;
							case 1:
								tile.ClearTile();
								tile.TileType = 0;
								tile.HasTile = true;
								break;
							case 2:
								tile.ClearTile();
								break;
							case 3:
								tile.ClearTile();
								break;
							case 4:
								tile.ClearTile();
								break;
							case 5:
								tile.ClearTile();
								break;
							case 6:
								break;
						}
					}
				}
			}

			for (int y = 0; y < BlocksArray.GetLength(0); y++)
			{
				for (int x = 0; x < BlocksArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;

					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);

						switch (BlocksArray[y, x])
						{
							case 0:
								break;
							case 1:
								WorldGen.PlaceTile(k, l, 2);
								break;
							case 2:
								WorldGen.PlaceObject(k, l, 215, true, 0);
								break;
							case 3:
								WorldGen.PlaceTile(k, l, ModContent.TileType<TentOpposite>());
								break;
							case 4:
								WorldGen.PlaceObject(k, l, 187, true, 26, 1, -1, -1);
								break;
							case 5:
								WorldGen.PlaceTile(k, l, 28);  // Pot
								tile.HasTile = true;
								break;
						}
					}
				}
			}
		}
		#endregion Campsite

		#region Crate Stashes
		private static void GenerateCrateStash()
		{
			while (true)
			{
				int hideoutX = WorldGen.genRand.Next(300, Main.maxTilesX - 300); // from 50 since there's a unaccessible area at the world's borders
				int hideoutY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 450);
				Tile tile = Main.tile[hideoutX, hideoutY];
				Point16 size = Point16.Zero;
				StructureHelper.Generator.GetDimensions("Structures/CrateStashRegular", SpiritMod.Instance, ref size);

				if (!tile.HasTile || tile.TileType != TileID.Stone || !GenVars.structures.CanPlace(new(hideoutX, hideoutY, size.X, size.Y)))
					continue;

				StructureHelper.Generator.GenerateStructure("Structures/CrateStashRegular", new Point16(hideoutX, hideoutY), SpiritMod.Instance);
				GenVars.structures.AddProtectedStructure(new(hideoutX, hideoutY, size.X, size.Y));
				break;
			}
		}

		private static void GenerateCrateStashJungle()
		{
			while (true)
			{
				int hideoutX = WorldGen.genRand.Next(300, Main.maxTilesX - 300); // from 50 since there's a unaccessible area at the world's borders
				int hideoutY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 450);
				Tile tile = Framing.GetTileSafely(hideoutX, hideoutY);
				Point16 size = Point16.Zero;
				StructureHelper.Generator.GetDimensions("Structures/CrateStashJungle", SpiritMod.Instance, ref size);

				if (!tile.HasTile || tile.TileType != 60 || !GenVars.structures.CanPlace(new(hideoutX, hideoutY, size.X, size.Y)))
					continue;

				StructureHelper.Generator.GenerateStructure("Structures/CrateStashJungle", new Point16(hideoutX, hideoutY), SpiritMod.Instance);
				GenVars.structures.AddProtectedStructure(new(hideoutX, hideoutY, size.X, size.Y));
				break;
			}
		}
		#endregion Crate Stashes

		#region Stone Dungeon
		private static void GenerateStoneDungeon()
		{
			while (true)
			{
				int hideoutX = WorldGen.genRand.Next(50, Main.maxTilesX - 200); // from 50 since there's a unaccessible area at the world's borders
				int hideoutY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);
				Tile tile = Framing.GetTileSafely(hideoutX, hideoutY);
				Point16 size = Point16.Zero;
				StructureHelper.Generator.GetDimensions("Structures/StoneDungeon", SpiritMod.Instance, ref size);

				if (!tile.HasTile || tile.TileType != TileID.Stone || !GenVars.structures.CanPlace(new(hideoutX, hideoutY, size.X, size.Y)))
					continue;

				StructureHelper.Generator.GenerateStructure("Structures/StoneDungeon", new Point16(hideoutX, hideoutY), SpiritMod.Instance);
				GenVars.structures.AddProtectedStructure(new(hideoutX, hideoutY, size.X, size.Y));
				break;
			}
		}
		#endregion Stone Dungeon

		#region Gem Stash
		private static void GenerateGemStash()
		{
			int[,] StashRoomMain = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,1,2,2,2,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,0,1,1,1,0,0},
				{0,0,1,4,4,0,0,0,0,0,4,4,4,4,4,4,0,0,0,0,0,4,4,4,1,0,0},
				{0,0,1,4,4,0,0,0,0,0,0,0,0,0,4,4,0,0,0,0,0,0,4,4,1,0,0},
				{0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,4,1,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,2,2,2,2,0,0,0},
				{0,0,1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,0},
				{0,0,1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,0},
				{0,0,0,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,0},
				{0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
			};

			int[,] StashRoomMain1 = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,1,2,2,2,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,0,1,1,1,0,0},
				{0,0,1,4,4,0,0,0,0,0,4,4,4,4,4,4,0,0,0,0,0,4,4,4,1,0,0},
				{0,0,1,4,4,0,0,0,0,0,0,0,0,0,4,4,0,0,0,0,0,0,4,4,1,0,0},
				{0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,4,1,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,2,2,2,2,0,0,0},
				{0,0,1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,7,0},
				{0,0,1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,7,7,0},
				{0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,0,7,7,7,7,0},
				{0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
			};

			int[,] StashMainWalls = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,1,2,2,2,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,0,1,1,1,0,0},
				{0,0,1,3,3,3,3,3,3,3,0,3,3,3,3,3,3,3,3,3,3,3,0,3,1,0,0},
				{0,0,1,0,0,3,3,3,3,3,3,0,3,3,3,3,3,3,3,3,3,3,0,3,1,0,0},
				{0,0,1,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,0,0},
				{0,0,0,3,0,3,3,3,3,3,0,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0},
				{0,0,1,3,3,3,3,3,3,0,3,3,3,3,3,3,3,3,3,3,3,0,0,3,1,0,0},
				{0,0,1,0,3,3,3,3,3,3,0,3,3,3,3,3,3,3,3,3,3,0,0,3,1,0,0},
				{0,0,0,3,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0},
				{0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
			};

			int[,] StashMainLoot = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,5,0,5,0,0,0,7,0,0,5,0,0,6,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			};

			int[,] StashMainLoot1 = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,4,0,5,0,0,0,0,9,0,0,5,0,0,6,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			};

			int[,] StashRoom1 = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,1,1,1,0,1,1,1,1,2,2,2,2,1,1,1,1,1,1,0,1,0,0,0,0},
				{0,0,0,1,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,4,4,4,1,0,0,0,0},
				{0,0,1,1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,4,1,0,0,0,0},
				{0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,1,0,0,0,0},
				{0,0,0,1,0,0,2,2,2,0,0,0,0,2,2,2,2,2,2,0,0,0,1,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0},
				{0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,1,1,1,1,1,1,1,0,1,2,2,2,2,1,1,1,1,1,0,1,0,0,0,0},
			};

			int[,] Stash1Walls = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,1,2,2,2,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,0,1,1,1,0,0},
				{0,0,1,3,3,3,3,3,3,3,0,3,3,3,3,3,3,3,3,3,3,3,0,3,3,0,0},
				{0,0,1,0,0,3,3,0,0,3,3,3,3,3,3,0,0,0,3,3,3,3,0,3,3,0,0},
				{0,0,1,0,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,3,3,3,3,3,1,0,0},
				{0,0,0,3,0,3,3,3,3,3,0,3,3,3,0,3,3,3,3,3,3,3,3,3,0,0,0},
				{0,0,1,3,0,3,3,3,3,0,3,3,3,3,0,3,3,3,3,3,3,0,0,3,3,0,0},
				{0,0,1,0,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,3,3,0,0,3,3,0,0},
				{0,0,0,3,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,3,0,0},
				{0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
			};

			int[,] Stash1Loot = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,5,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,5,0,0,8,0,0,0,5,0,5,0,5,0,8,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			};

			Point center = new Point();
			int width = (StashRoomMain.GetLength(1) / 2) + 5;
			int height = (StashRoomMain.GetLength(0) / 2) + 4;
			Rectangle positionRange = new Rectangle(Main.spawnTileX - 800, (int)(Main.worldSurface + 40), 1600, 280);
			BlacklistBlocks(80, width, height, positionRange, ref center, out bool failed);

			if (failed) //Dont generate if tried too many times
				return;

			GenVars.structures.AddProtectedStructure(new Rectangle(center.X - width / 2, center.Y - height / 2, width, height), 2);

			// place the hideout
			if (WorldGen.genRand.NextBool(2))
				PlaceGemStash(center.X, center.Y, StashRoomMain, StashMainWalls, StashMainLoot);
			else
				PlaceGemStash(center.X, center.Y, StashRoomMain1, StashMainWalls, StashMainLoot1);

			if (WorldGen.genRand.NextBool(2))
			{
				int x = center.X + WorldGen.genRand.Next(-5, 5);
				PlaceGemStash(x, center.Y - 8, StashRoom1, Stash1Walls, Stash1Loot);
				GenVars.structures.AddProtectedStructure(new Rectangle(x - width / 2, center.Y - height / 2 - 8, width, height), 2);
			}
		}

		private static void PlaceGemStash(int i, int j, int[,] BlocksArray, int[,] WallsArray, int[,] LootArray)
		{
			for (int y = 0; y < WallsArray.GetLength(0); y++)
			{
				for (int x = 0; x < WallsArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;

					if (WorldGen.InWorld(k, l, 30))
					{
						switch (WallsArray[y, x])
						{
							case 0:
								break;
							case 1:
								WorldGen.KillWall(k, l);
								Framing.GetTileSafely(k, l).ClearTile();
								break;
							case 2:
								WorldGen.KillWall(k, l);
								Framing.GetTileSafely(k, l).ClearTile();
								break;
							case 3:
								WorldGen.KillWall(k, l);
								Framing.GetTileSafely(k, l).ClearTile();
								break;
						}
					}
				}
			}

			for (int y = 0; y < BlocksArray.GetLength(0); y++)
			{
				for (int x = 0; x < BlocksArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;

					if (WorldGen.InWorld(k, l, 30))
					{
						switch (BlocksArray[y, x])
						{
							case 0:
								break;

							case 1:
								Framing.GetTileSafely(k, l).ClearTile();
								break;

							case 2:
								Framing.GetTileSafely(k, l).ClearTile();
								break;

							case 3:
								Framing.GetTileSafely(k, l).ClearTile();
								break;
						}
					}
				}
			}

			for (int y = 0; y < BlocksArray.GetLength(0); y++)
			{
				for (int x = 0; x < BlocksArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;

					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);

						switch (BlocksArray[y, x])
						{
							case 0:
								break;

							case 1:
								WorldGen.PlaceTile(k, l, 30);
								tile.HasTile = true;
								break;

							case 2:
								WorldGen.PlaceTile(k, l, 19);
								tile.HasTile = true;
								break;

							case 3:
								WorldGen.PlaceTile(k, l, 63);
								tile.HasTile = true;
								break;

							case 4:
								WorldGen.PlaceTile(k, l, 51);
								tile.HasTile = true;
								break;

							case 7:
								WorldGen.PlaceTile(k, l, 64);
								tile.HasTile = true;
								break;
						}
					}
				}
			}

			for (int y = 0; y < WallsArray.GetLength(0); y++)
			{
				for (int x = 0; x < WallsArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;

					if (WorldGen.InWorld(k, l, 30))
					{
						switch (WallsArray[y, x])
						{
							case 0:
								break;

							case 3:
								WorldGen.PlaceWall(k, l, 27);
								break;
						}
					}
				}
			}

			for (int y = 0; y < LootArray.GetLength(0); y++)
			{
				for (int x = 0; x < LootArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;

					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);

						switch (LootArray[y, x])
						{
							case 0:
								break;

							case 4:
								WorldGen.PlaceObject(k, l, TileID.FishingCrate);  // Crate
								break;

							case 5:
								WorldGen.PlaceTile(k, l, TileID.Pots);  // Pot
								tile.HasTile = true;
								break;

							case 6:
								int objects;
								if (WorldGen.genRand.NextBool(3))
								{
									objects = TileID.Statues;
								}
								else if (WorldGen.genRand.NextBool(2))
								{
									objects = TileID.Anvils;
								}
								else if (WorldGen.genRand.NextBool(4))
								{
									objects = TileID.Pianos;
								}
								else if (WorldGen.genRand.NextBool(4))
								{
									objects = TileID.WorkBenches;
								}
								else
								{
									objects = TileID.Pots;
								}

								WorldGen.PlaceObject(k, l, (ushort)objects);  // Misc
								break;

							case 7:
								WorldGen.PlaceObject(k, l - 1, ModContent.TileType<GemsPickaxeSapphire>());  // Special Pick		
								break;

							case 8:
								if (WorldGen.genRand.NextBool(3))
								{
									objects = TileID.Statues;
								}
								else if (WorldGen.genRand.NextBool(2))
								{
									objects = TileID.Anvils;
								}
								else if (WorldGen.genRand.NextBool(4))
								{
									objects = TileID.Pianos;
								}
								else if (WorldGen.genRand.NextBool(4))
								{
									objects = TileID.WorkBenches;
								}
								else
								{
									objects = TileID.Pots;
								}

								WorldGen.PlaceObject(k, l, (ushort)objects);  // Another Misc Obj
								break;

							case 9:
								WorldGen.PlaceObject(k, l - 1, ModContent.TileType<GemsPickaxeRuby>());  // Special Pick		
								break;
						}
					}
				}
			}
		}
		#endregion Gem Stash

		#region Bone Island
		public static void GenerateBoneIsland()
		{
			int var = WorldGen.genRand.Next(2);
			string structure = "Structures/BoneIsland" + var;

			Point16 size = default;
			StructureHelper.Generator.GetDimensions(structure, SpiritMod.Instance, ref size);
			Point pos = FindBoneIslandPlacement(size, WorldGen.remixWorldGen); // Select a place in the inner 4/6ths of the world
			StructureHelper.Generator.GenerateStructure(structure, new Point16(pos.X, pos.Y), SpiritMod.Instance);
			GenVars.structures.AddProtectedStructure(new Rectangle(pos.X, pos.Y, size.X, size.Y), 8);

			if (var == 1 && Main.tile[pos.X + 118, pos.Y + 35].TileType == TileID.TatteredWoodSign)
			{
				int sign = Sign.ReadSign(pos.X + 118, pos.Y + 35, true);

				if (sign != -1)
				{
					WeightedRandom<int> lines = new();
					lines.Add(0, 0.6f);
					lines.Add(1, 0.09f);
					lines.Add(2, 0.01f);
					lines.Add(3, 0.01f);
					lines.Add(4, 0.29f);
					Sign.TextSign(sign, Language.GetTextValue("Mods.SpiritMod.AvianIslandSigns" + lines));
				}
			}
		}

		private static Point FindBoneIslandPlacement(Point16 islandSize, bool hugIsland = false)
		{
			int totalAttempts = -1;

			if (ModLoader.HasMod("Remnants"))
				return RemnantsFindBoneIslandPlacement(islandSize);

			while (true)
			{
			fullRestart:
				totalAttempts++;

				if (totalAttempts > 3000)
					break;

				if (houseLocations.Count == 0)
					break;

				Point pos = WorldGen.genRand.Next(houseLocations);

				while (true)
				{
					if (!hugIsland)
					{
						int repositionCount = 0;

						pos = new Point(WorldGen.genRand.Next(Main.maxTilesX / 6, (int)(Main.maxTilesX / 1.16f)), WorldGen.genRand.Next(80, (int)(Main.worldSurface * 0.34)));

						while (pos.X > Main.maxTilesX / 2 - 360 && pos.X < Main.maxTilesX / 2 + 240)
						{
							pos.X = WorldGen.genRand.Next(Main.maxTilesX / 6, (int)(Main.maxTilesX / 1.16f));
							repositionCount++;

							if (repositionCount > 500) // Cut off loop if repeated a ton without success
								goto fullRestart;
						}
					}

					Point realPos = pos;

					if (hugIsland)
					{
						int xOffset = WorldGen.genRand.NextBool() ? WorldGen.genRand.Next(50, 60) : -WorldGen.genRand.Next(70, 90);
						realPos = new Point(pos.X + xOffset, pos.Y + WorldGen.genRand.Next(10, 25));
					}

					bool failed = false;

					for (int i = 0; i < islandSize.X; ++i)
					{
						if (failed)
							break;

						for (int j = 0; j < islandSize.Y; ++j)
						{
							Tile tile = Framing.GetTileSafely(realPos.X + i, realPos.Y + j);
							if (tile.HasTile)
							{
								failed = true; //Retry if this space is taken up
								break;
							}
						}
					}

					if (!GenVars.structures.CanPlace(new Rectangle(realPos.X, realPos.Y, islandSize.X, islandSize.Y)))
						failed = true;

					if (failed)
						goto fullRestart;

					if (hugIsland)
						houseLocations.Remove(pos);
					return realPos;
				}
			}

			return new Point(0, 0);
		}

		private static Point RemnantsFindBoneIslandPlacement(Point16 islandSize)
		{
			Point position;

			do
			{
				position = new Point(WorldGen.genRand.Next(300, Main.maxTilesX - 300), WorldGen.genRand.Next(50, (int)(Main.worldSurface * 0.35f)));
			} while (!WorldMethods.AreaClear(position.X, position.Y, islandSize.X, islandSize.Y) ||
				!GenVars.structures.CanPlace(new Rectangle(position.X, position.Y, islandSize.X, islandSize.Y)));

			return position;
		}
		#endregion Bone Island

		#region Pagoda
		private static void GeneratePagoda()
		{
			if (MyWorld.asteroidSide == 0)
				MyWorld.pagodaLocation.X = Main.maxTilesX - WorldGen.genRand.Next(200, 350);
			else
				MyWorld.pagodaLocation.X = WorldGen.genRand.Next(200, 350);

			MyWorld.pagodaLocation.Y = (int)(Main.worldSurface / 5.0);
			StructureHelper.Generator.GenerateStructure("Structures/Pagoda", new(MyWorld.pagodaLocation.X, MyWorld.pagodaLocation.Y), SpiritMod.Instance);

		}
		#endregion Pagoda

		#region Ziggurat
		private static void GenerateZiggurat()
		{
			int[,] ZigguratShape = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,3,3,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,3,3,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,3,3,3,3,3,3,3,3,3,3,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,1,1,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,1,1,0,0,0,0,0},
				{0,0,0,0,0,1,1,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,1,1,0,0,0,0,0},
				{0,0,0,0,0,1,1,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,1,1,0,0,0,0,0},
				{0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0},
				{0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0},
				{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			};

			int[,] ZigguratWalls = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3,3,3,3,3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3,3,3,3,3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3,3,3,3,3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,1,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,1,0,0,0,0,0,0},
				{0,0,0,0,0,0,1,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,1,0,0,0,0,0,0},
				{0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0},
				{0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			};

			int[,] ZigguratLoot = new int[,]
			{
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,3,3,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,5,3,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,6,3,3,3,3,3,3,3,3,6,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,3,3,3,3,4,3,3,3,3,3,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,6,3,3,3,3,3,3,3,3,3,3,6,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,1,1,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,1,1,0,0,0,0,0},
				{0,0,0,0,0,1,1,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,1,1,0,0,0,0,0},
				{0,0,0,0,0,1,1,1,1,1,1,1,1,3,3,7,3,7,3,3,8,3,3,3,7,3,7,3,7,1,1,1,1,1,1,1,1,0,0,0,0,0},
				{0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0},
				{0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0},
				{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			};

			bool placed = false;
			while (!placed)
			{
				// Select a place in the first 6th of the world
				int hideoutX = WorldGen.genRand.Next(Main.maxTilesX / 6, Main.maxTilesX / 6 * 5); // from 50 since there's a unaccessible area at the world's borders
																						   // 50% of choosing the last 6th of the world
				if (WorldGen.genRand.NextBool())
				{
					hideoutX = Main.maxTilesX - hideoutX;
				}

				int hideoutY = 0;
				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(hideoutX, hideoutY) && hideoutY <= Main.worldSurface)
				{
					hideoutY++;
				}

				// If we went under the world's surface, try again
				if (hideoutY > Main.worldSurface)
				{
					continue;
				}

				Tile tile = Main.tile[hideoutX, hideoutY];
				// If the type of the tile we are placing the hideout on doesn't match what we want, try again
				if (tile.TileType != TileID.Sand && tile.TileType != TileID.Ebonsand && tile.TileType != TileID.Crimsand && tile.TileType != TileID.Sandstone)
				{
					continue;
				}

				// place the hideout
				PlaceZiggurat(hideoutX, hideoutY - 1, ZigguratShape, ZigguratWalls, ZigguratLoot);
				placed = true;
			}
		}

		private static void PlaceZiggurat(int i, int j, int[,] BlocksArray, int[,] WallsArray, int[,] LootArray)
		{
			for (int y = 0; y < BlocksArray.GetLength(0); y++)
			{
				for (int x = 0; x < BlocksArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;
					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);
						switch (BlocksArray[y, x])
						{
							case 0:
								break;
							case 1:
								WorldGen.KillWall(k, l);
								Framing.GetTileSafely(k, l).ClearTile();
								break;
							case 2:
								WorldGen.KillWall(k, l);
								Framing.GetTileSafely(k, l).ClearTile();
								break;
							case 3:
								WorldGen.KillWall(k, l);
								Framing.GetTileSafely(k, l).ClearTile();
								break;
						}
					}
				}
			}

			for (int y = 0; y < WallsArray.GetLength(0); y++)
			{
				for (int x = 0; x < WallsArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;
					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);
						switch (WallsArray[y, x])
						{
							case 0:
								break;
							case 1:
								WorldGen.KillWall(k, l);
								Framing.GetTileSafely(k, l).ClearTile();
								break;
							case 2:
								WorldGen.KillWall(k, l);
								Framing.GetTileSafely(k, l).ClearTile();
								break;
							case 3:
								WorldGen.KillWall(k, l);
								Framing.GetTileSafely(k, l).ClearTile();
								break;
						}
					}
				}
			}

			for (int y = 0; y < BlocksArray.GetLength(0); y++)
			{
				for (int x = 0; x < BlocksArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;
					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);
						switch (BlocksArray[y, x])
						{
							case 0:
								break;
							case 1:
								WorldGen.PlaceTile(k, l, 151);
								tile.HasTile = true;
								break;
							case 2:
								WorldGen.PlaceTile(k, l, 152);
								tile.HasTile = true;
								break;
						}
					}
				}
			}

			for (int y = 0; y < WallsArray.GetLength(0); y++)
			{
				for (int x = 0; x < WallsArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;
					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);
						switch (WallsArray[y, x])
						{
							case 0:
								break;
							case 1:
								WorldGen.PlaceWall(k, l, 34);
								break;
							case 2:
								WorldGen.PlaceWall(k, l, 35);
								break;
							case 3:
								WorldGen.PlaceWall(k, l, 34);
								break;
						}
					}
				}
			}

			for (int y = 0; y < LootArray.GetLength(0); y++)
			{
				for (int x = 0; x < LootArray.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;
					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);
						switch (LootArray[y, x])
						{
							case 0:
								break;
							case 4:
								WorldGen.PlaceObject(k, l, ModContent.TileType<Tiles.Ambient.ScarabIdol>());
								break;
							case 5:
								WorldGen.PlaceChest(k, l, (ushort)ModContent.TileType<GoldScarabChest>(), false, 0);
								break;
							case 6:
								WorldGen.PlaceObject(k, l, 91);
								break;
							case 7:
								WorldGen.PlaceTile(k, l, 28);
								break;
							case 8:
								WorldGen.PlaceTile(k, l, 102);
								break;
						}
					}
				}
			}
		}
		#endregion Ziggurat

		#region GENPASS: ASTEROIDS
		public static void AsteroidsPass(GenerationProgress progress, GameConfiguration config)
		{
			progress.Message = Language.GetTextValue("Mods.SpiritMod.WorldGen.Asteroids");
			int width = 200 + (int)(((Main.maxTilesX / 4200f) - 1) * 75); //Automatically scales based on world size
			int height = 50 + (int)(((Main.maxTilesX / 4200f) - 1) * 15);
			int x = width + 80;
			MyWorld.asteroidSide = 0;

			if (WorldGen.genRand.NextBool())
			{
				x = Main.maxTilesX - (width + 80);
				MyWorld.asteroidSide = 1;
			}

			int y = height + WorldGen.genRand.Next(36, 50); //If you want to change the top of the asteroid biome, change this
			PlaceAsteroids(x, y, width, height);
		}
		#endregion GENPASS: ASTEROIDS

		#region Asteroid Methods
		private static void PlaceAsteroids(int i, int j, int width, int height)
		{
			int numberOfAsteroids = 33 + (int)(((Main.maxTilesX / 4200f) - 1) * 20); //easy world size scaling woo
			int numJunkPiles = 15 + (int)(((Main.maxTilesX / 4200f) - 1) * 8);
			int numberOfOres = 140 + (int)(((Main.maxTilesX / 4200f) - 1) * 80);
			int numberOfBigs = 1;

			if (Main.maxTilesX == 6400) //didn't want to redo this since it seems important, but I did fix it for XL worlds
				numberOfBigs = 2;
			else if (Main.maxTilesX >= 8400)
				numberOfBigs = 4;

			for (int k = 0; k < numberOfAsteroids; k++) //small asteroids
			{
				int angle = WorldGen.genRand.Next(360);
				float xsize = (float)(WorldGen.genRand.Next(100, 120)) / 100;
				float ysize = (float)(WorldGen.genRand.Next(100, 120)) / 100;
				int size = WorldGen.genRand.Next(6, 7);
				int x = i + (int)(WorldGen.genRand.Next(width) * Math.Sin(angle * (Math.PI / 180))) + WorldGen.genRand.Next(-100, 100);
				int y = j + (int)(WorldGen.genRand.Next(height) * Math.Cos(angle * (Math.PI / 180))) + WorldGen.genRand.Next(-10, 15);
				PlaceBlob(x, y, xsize, ysize, size, ModContent.TileType<Asteroid>(), 50, true, ModContent.WallType<AsteroidWall>());
			}

			for (int k = 0; k < numJunkPiles; k++) //junkPiles
			{
				int angle = WorldGen.genRand.Next(360);
				float xsize = (float)(WorldGen.genRand.Next(100, 120)) / 100;
				float ysize = (float)(WorldGen.genRand.Next(100, 120)) / 100;
				int size = WorldGen.genRand.Next(3, 4);
				int x = i + (int)(WorldGen.genRand.Next(width) * Math.Sin(angle * (Math.PI / 180))) + WorldGen.genRand.Next(-100, 100);
				int y = j + (int)(WorldGen.genRand.Next(height) * Math.Cos(angle * (Math.PI / 180))) + WorldGen.genRand.Next(-10, 15);
				PlaceBlob(x, y, xsize, ysize, size, ModContent.TileType<SpaceJunkTile>(), 50);
			}

			for (int k = 0; k < numberOfBigs; k++) //big asteroids
			{
				int x = i + (int)(WorldGen.genRand.Next(0 - width, width) / 1.5f);
				int y = j + WorldGen.genRand.Next(0 - height, height);
				float xsize = (float)(WorldGen.genRand.Next(75, 133)) / 100;
				float ysize = (float)(WorldGen.genRand.Next(75, 133)) / 100;
				int size = WorldGen.genRand.Next(11, 17);
				PlaceBlob(x, y, xsize, ysize, size, ModContent.TileType<BigAsteroid>(), 10, true, ModContent.WallType<AsteroidWall>());
			}

			for (int k = 0; k < numberOfOres; k++) //ores
			{
				int angle = WorldGen.genRand.Next(360);
				int x = i + (int)(WorldGen.genRand.Next(width) * Math.Sin(angle * (Math.PI / 180))) + WorldGen.genRand.Next(-100, 100);
				int y = j + (int)(WorldGen.genRand.Next(height) * Math.Cos(angle * (Math.PI / 180))) + WorldGen.genRand.Next(-10, 15);
				ushort ore = OreRoller((ushort)ModContent.TileType<Glowstone>());
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(2, 10), 2, ore, false, 0f, 0f, false, true);
			}

			StructureHelper.Generator.GenerateStructure("Structures/StarAltar", new Point16(i + width + WorldGen.genRand.Next(-20, 20), j + WorldGen.genRand.Next((int)(height * 0.33f), (int)(height * 0.66f))), SpiritMod.Instance);

			const int MaxChestTries = 10000;
			int chestTries = 0;
			int chestSuccesses = 0;

			while (chestTries < MaxChestTries && chestSuccesses < 4) // Chest spawning
			{
				int x = i + WorldGen.genRand.Next(0 - width, width);
				int y = j + WorldGen.genRand.Next(0 - height, height);

				if (WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<Tiles.Furniture.SpaceJunk.AsteroidChest>(), false, 0) != -1)
				{
					chestSuccesses++;
					chestTries = 0;
				}

				chestTries++;
			}

			ushort asteroidType = (ushort)ModContent.TileType<Asteroid>();
			ushort junkType = (ushort)ModContent.TileType<SpaceJunkTile>();

			for (int smoothX = i - width; smoothX < i + width; smoothX++) // Super lazy basic smoothing pass
			{
				for (int smoothY = j - height; smoothY < j + height; smoothY++)
				{
					if (!WorldGen.InWorld(smoothX, smoothY)) 
						continue;

					Tile tile = Framing.GetTileSafely(smoothX, smoothY);
					if (tile.HasTile && (tile.TileType == junkType || tile.TileType == asteroidType) && WorldGen.genRand.NextBool(2))
						Tile.SmoothSlope(smoothX, smoothY);
				}
			}
		}

		private static void PlaceBlob(int x, int y, float xsize, float ysize, int size, int type, int roundness, bool placewall = false, int walltype = 0)
		{
			int distance = size;
			for (int i = 0; i < 360; i++)
			{
				if (360 - i <= Math.Abs(size - distance) / Math.Sqrt(size) * 50)
				{
					if (size > distance)
					{
						distance++;
					}
					else
					{
						distance--;
					}
				}
				else
				{
					int increase = WorldGen.genRand.Next(roundness);
					if (increase == 0 && distance > 3)
					{
						distance--;
					}

					if (increase == 1)
					{
						distance++;
					}
				}

				int offsetX = (int)(Math.Sin(i * (Math.PI / 180)) * distance * xsize);
				int offsetY = (int)(Math.Cos(i * (Math.PI / 180)) * distance * ysize);
				WorldExtras.PlaceLine(x, y, x + offsetX, y + offsetY, type, placewall, walltype);
			}
		}

		private static ushort OreRoller(ushort glowstone)
		{
			ushort iron = WorldExtras.GetOreCounterpart(WorldGen.SavedOreTiers.Iron);
			ushort silver = WorldExtras.GetOreCounterpart(WorldGen.SavedOreTiers.Silver);
			ushort gold = WorldExtras.GetOreCounterpart(WorldGen.SavedOreTiers.Gold);

			int roll = WorldGen.genRand.Next(1070);
			if (roll < 250)
				return WorldExtras.GetOreCounterpart(iron);
			else if (roll < 400)
				return WorldExtras.GetOreCounterpart(silver);
			else if (roll < 600)
				return WorldExtras.GetOreCounterpart(gold);
			else
				return glowstone;
		}
		#endregion Asteroids

		#region GENPASS: PILES/AMBIENT
		public static void PilesPass(GenerationProgress progress, GameConfiguration config)
		{
			progress.Message = Language.GetTextValue("Mods.SpiritMod.WorldGen.AmbientObjects");

			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY * 78) * 15E-05); k++)
			{
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 130);
				if (Main.tile[x, y] != null && Main.tile[x, y].HasTile)
				{
					if (Main.tile[x, y].TileType == TileID.Granite)
						WorldGen.OreRunner(x, y, WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), (ushort)ModContent.TileType<GraniteOre>());

					if (Main.tile[x, y].TileType == TileID.Marble)
						WorldGen.OreRunner(x, y, WorldGen.genRand.Next(5, 8), WorldGen.genRand.Next(4, 9), (ushort)ModContent.TileType<MarbleOre>());
				}
			}

			int repeats = -1;
			int maxRepeats = 3;
			for (int i = 0; i < (maxRepeats * GlobalExtensions.WorldSize); ++i)
			{
				if (repeats >= (maxRepeats * GlobalExtensions.WorldSize))
					continue;

				repeats++;
				int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
				int y = WorldGen.genRand.Next(Main.UnderworldLayer, Main.maxTilesY);

				if (Framing.GetTileSafely(x, y).WallType == WallID.ObsidianBrickUnsafe)
				{
					int type = ModContent.TileType<TheBadPainting>();
					TileObjectData data = TileObjectData.GetTileData(type, 0);

					int originY = data is not null ? data.Origin.Y : 1;
					int height = data is not null ? data.Height : 1;

					if (WorldGen.PlaceObject(x, y - height + originY, type, true))
						NetMessage.SendObjectPlacement(-1, x, y - height + originY, type, 0, 0, -1, -1);

				}
				else 
					continue;
			} //Place "The Bad" paintings

			int[] mushSet = GlobalExtensions.TileSet<WhiteMushroom2x2, WhiteMushroom2x3, RedMushroom1x1, RedMushroom2x2, RedMushroom3x2, BrownMushrooms, BrownMushroomLarge>();
			AddDecorSpam("Mushrooms", mushSet, TileObjectData.GetTileData(ModContent.TileType<WhiteMushroom2x2>(), 0).AnchorValidTiles, 500, (int)Main.worldSurface, Main.maxTilesY - 200);
			AddDecorSpam("OreDeposits", new int[] { ModContent.TileType<OreDeposits>() }, TileSets.Mosses.With(TileID.Stone), 130, (int)Main.worldSurface, Main.maxTilesY - 200);
			AddDecorSpam("LargeRock", new int[] { ModContent.TileType<LargeRock>() }, TileSets.Mosses.With(TileID.Stone), 55, (int)Main.worldSurface, Main.maxTilesY - 200);
			AddDecorSpam("BlueShards", new[] { ModContent.TileType<BlueShardBig>() }, GlobalExtensions.TileSet<Asteroid, BigAsteroid>(), 14, 42, (int)Main.worldSurface);
			int[] sculptures = GlobalExtensions.TileSet<IceWheezerPassive, IceFlinxPassive, IceBatPassive, IceVikingPassive, IceWheezerHostile, IceFlinxHostile, IceBatHostile, IceVikingHostile>();
			AddDecorSpam("IceStatues", sculptures, new int[] { TileID.IceBlock, TileID.SnowBlock }, 120, (int)GenVars.rockLayer, Main.maxTilesY);
			AddPileSpam(); //All ore pile decorspam info in a method for clarity
			AddDecorSpam("ExplosiveBarrels", new int[] { ModContent.TileType<ExplosiveBarrelTile>() }, new int[] { TileID.Stone }, 75, (int)GenVars.rockLayer, Main.maxTilesY - 200);
			int[] snowFoliage = GlobalExtensions.TileSet<SnowBush1, SnowBush2, SnowBush3, TundraBerries1x2, TundraBerries2x2>();
			AddDecorSpam("SnowFoliage", snowFoliage, new int[] { TileID.SnowBlock, TileID.IceBlock }, 140, (int)Main.worldSurface - 100, (int)Main.worldSurface + 30);
			AddDecorSpam("IceCubes", GlobalExtensions.TileSet<IceCube1, IceCube2, IceCube3>(), new int[] { TileID.SnowBlock, TileID.IceBlock }, 120, (int)Main.worldSurface - 100, (int)Main.worldSurface + 30);
			AddDecorSpam("Statues", new int[] { ModContent.TileType<ForgottenKingStatue>() }, TileSets.Mosses.With(TileID.Stone, TileID.Obsidian), 15, (int)Main.worldSurface + 100, (int)Main.maxTilesY - 200);
			AddDecorSpam("OreCarts", new int[] { ModContent.TileType<OreCarts>() }, new int[] { TileID.MinecartTrack }, 80, (int)Main.worldSurface + 100, (int)Main.maxTilesY - 200);

			float vol = Main.soundVolume; //Dumb sound workaround because it doesn't want to SHUT UP >:(
			Main.soundVolume = 0;
			PopulateSpam(progress);
			Main.soundVolume = vol;
		}

		private static void AddPileSpam()
		{
			int type = WorldGen.SavedOreTiers.Copper == TileID.Copper ? ModContent.TileType<CopperPile>() : ModContent.TileType<TinPile>();
			AddDecorSpam("CopperPiles", new int[1] { type }, new int[] { TileID.Stone }, 125, (int)GenVars.rockLayer, Main.maxTilesY - 200);
			type = WorldGen.SavedOreTiers.Iron == TileID.Iron ? ModContent.TileType<IronPile>() : ModContent.TileType<LeadPile>();
			AddDecorSpam("IronPiles", new int[1] { type }, new int[] { TileID.Stone }, 90, (int)GenVars.rockLayer, Main.maxTilesY - 200);
			type = WorldGen.SavedOreTiers.Silver == TileID.Silver ? ModContent.TileType<SilverPile>() : ModContent.TileType<TungstenPile>();
			AddDecorSpam("SilverPiles", new int[1] { type }, new int[] { TileID.Stone }, 90, (int)GenVars.rockLayer, Main.maxTilesY - 200);
			type = WorldGen.SavedOreTiers.Gold == TileID.Gold ? ModContent.TileType<GoldPile>() : ModContent.TileType<PlatinumPile>();
			AddDecorSpam("GoldPiles", new int[1] { type }, new int[] { TileID.Stone }, 75, (int)GenVars.rockLayer, Main.maxTilesY - 200);
		}

		private static void AddDecorSpam(string name, int[] types, int[] ground, int baseReps, int high, int low, bool forced = true)
		{
			DecorSpamData data = new DecorSpamData(name, types, ground, baseReps, (high, low), forced);
			decorSpam.Add(data);
		}

		private static void PopulateSpam(GenerationProgress progress)
		{
			int maxReps = 0;
			Dictionary<string, int> repeatsByName = new Dictionary<string, int>();

			foreach (var item in decorSpam)
			{
				if (item.BaseRepeats > maxReps)
					maxReps = item.BaseRepeats;

				repeatsByName.Add(item.Name, 0);
			}

			for (int i = 0; i < maxReps * GlobalExtensions.WorldSize; ++i)
			{
				progress.Set(i / (maxReps * (double)GlobalExtensions.WorldSize));

				foreach (var item in decorSpam)
				{
					if (repeatsByName[item.Name] >= item.RealRepeats)
						continue;

					int individualRetries = 0;

				retry:
					individualRetries++;

					if (individualRetries > maxReps)
						continue;

					int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
					int y = WorldGen.genRand.Next(item.RangeY.high, item.RangeY.low);
					Tile tile = Main.tile[x, y];

					if (tile.HasTile && item.ValidGround.Contains(tile.TileType))
					{
						int type = WorldGen.genRand.Next(item.Types);
						TileObjectData data = TileObjectData.GetTileData(type, 0);

						int originY = data is not null ? data.Origin.Y : 1;
						int height = data is not null ? data.Height : 1;
						int styleRange = data is not null ? data.RandomStyleRange : 1;
						int style = WorldGen.genRand.Next(styleRange);

						bool didPlace = WorldGen.PlaceObject(x, y - height + originY, type, true, style);

						if (didPlace)
						{
							repeatsByName[item.Name]++;
							NetMessage.SendObjectPlacement(-1, x, y - height + originY, type, style, 0, -1, -1);
						}
						else
						{
							if (!item.Forced)
							{
								repeatsByName[item.Name]++;
								continue;
							}

							goto retry; //worldgen code is a cesspool you can deal with a little goto
						}
					}
					else
						goto retry;
				}
			}

			decorSpam.Clear();
		}
		#endregion GENPASS: Piles/Ambient
	}
}