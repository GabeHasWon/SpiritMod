using SpiritMod.NPCs.Town;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.World
{
	public class GoblinTower
	{
		public static int OffsetX => -3;
		public static int OffsetY => -43;
		public static int[,] Tiles => new int[,] {
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0},
		//	{0,0,0,0,0,6,6,6,6,0,0,0,0,0,0,0,0,0,0,6,6,6,6,0,0,0,0,0},
		//	{0,0,0,0,6,6,6,1,0,0,0,0,0,0,0,0,0,0,0,4,1,6,6,6,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,2,2,2,2,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,0,0,0,0,0,0,2,2,2,2,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,3,3,3,3,3,1,4,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,3,3,3,3,1,4,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,3,3,3,1,2,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,2,1,4,2,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,2,2,2,1,1,2,2,2,2,2,1,3,3,3,3,3,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,0,0,0,0,0,0,0,0,2,4,1,3,3,3,3,0,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,0,0,0,0,0,0,0,2,0,4,1,3,3,3,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,0,0,0,0,0,0,0,0,0,4,1,2,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,1,4,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,4,0,0,0,0,0,2,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,4,0,0,0,0,2,0,0,0,0,0,0,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,0,0,0,0,4,0,0,0,2,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0},
		//	{0,0,3,3,3,3,3,1,2,2,2,2,0,0,0,0,2,2,2,2,1,0,0,0,0,0,0,0},
		//	{0,0,0,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,4,0,2,1,4,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,4,0,0,1,4,0,0,0,0,0,0,0,0,0,0,4,1,0,0,0,0,0,0,0},
		//	{0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,4,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
		//	{0,0,0,0,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0},
		//	{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
		//	{0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
		//	{0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0}
		};

		private static int shingleColor;

		public static bool Generate()
		{
			shingleColor = WorldGen.genRand.NextBool() ? TileID.RedDynastyShingles : TileID.BlueDynastyShingles;
			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 100000) {
				// Select a place in the first 6th of the world, avoiding the oceans
				int towerX = WorldGen.genRand.Next(300, Main.maxTilesX / 6); // from 50 since there's a unaccessible area at the world's borders
																			 // 50% of choosing the last 6th of the world
																			 // Choose which side of the world to be on randomly
				if (WorldGen.genRand.NextBool())
					towerX = Main.maxTilesX - towerX;

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int towerY = (int)Main.worldSurface - 200;

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(towerX, towerY) && towerY <= Main.worldSurface)
					towerY++;

				// If we went under the world's surface, try again
				if (towerY > Main.worldSurface)
					continue;

				Tile tile = Main.tile[towerX, towerY];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if (!(tile.TileType == TileID.Dirt
					|| tile.TileType == TileID.Grass
					|| tile.TileType == TileID.Stone
					|| tile.TileType == TileID.Mud
					|| tile.TileType == TileID.CrimsonGrass
					|| tile.TileType == TileID.CorruptGrass
					|| tile.TileType == TileID.JungleGrass
					|| tile.TileType == TileID.Sand
					|| tile.TileType == TileID.Crimsand
					|| tile.TileType == TileID.Ebonsand))
					continue;

				// Don't place the tower if the area isn't flat
				if (!MyWorld.CheckFlat(towerX, towerY, Tiles.GetLength(1), 3))
					continue;

				// place the tower
				StructureHelper.Generator.GenerateStructure("World/Structures/GoblinTower", new Point16(towerX + OffsetX, towerY + OffsetY), SpiritMod.Instance);

				// place the Gambler
				int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				Main.npc[num].homeTileX = -1;
				Main.npc[num].homeTileY = -1;
				Main.npc[num].direction = 1;
				Main.npc[num].homeless = true;
				placed = true;
			}

			if (!placed) 
				SpiritMod.Instance.Logger.Error("Worldgen: FAILED to place Goblin Tower, ground not flat enough?");
			return placed;
		}
	}
}