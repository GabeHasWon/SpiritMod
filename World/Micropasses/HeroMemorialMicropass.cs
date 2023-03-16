using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using SpiritMod.Tiles.Furniture;

namespace SpiritMod.World.Micropasses;

internal class HeroMemorialMicropass : Micropass
{
	public override string WorldGenName => "A Hero's Memorial";

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex)
	{
		if (!WorldGen.genRand.NextBool(12))
			return -1;

		afterIndex = false;
		return passes.FindIndex(genpass => genpass.Name.Equals("Flowers"));
	}

	public override void Run(GenerationProgress progress, GameConfiguration config)
	{
		progress.Message = "Spirit Mod Microstructures: A Hero's Memorial";

	retry:
		int worldMod = (int)(200 * (Main.maxTilesX / 4200f));
		int x = WorldGen.genRand.Next(200, (Main.maxTilesX / 2) - worldMod) / 2;

		if (WorldGen.genRand.NextBool(2))
			x = WorldGen.genRand.Next((Main.maxTilesX / 2) + worldMod, Main.maxTilesX - 200) / 2;

		int y = (int)(Main.worldSurface * 0.4f);

		while (!Main.tile[x, y].HasTile || Main.tile[x, y].TileType != TileID.Grass) //Loop to valid ground
		{
			y++;

			if (y > Main.worldSurface + 20)
				goto retry;
		}

		y -= 4;

		int count = 0;
		Point16 dims = new();
		StructureHelper.Generator.GetDimensions("Structures/HeroMemorial", ModLoader.GetMod("SpiritMod"), ref dims);

		if (NearOtherChests(x + 11, y + 2, 12, 3)) //Check for nearby chests
			goto retry;

		for (int i = x; i < x + dims.X; ++i) //Check for ground
			for (int j = y + dims.Y; j < y + dims.Y + 3; ++j)
				if (WorldGen.SolidOrSlopedTile(i, j))
					count++;

		if (count < 40) //Retry if there's not enough ground
			goto retry;

		count = 0;

		for (int i = x; i < x + dims.X; ++i) //Check for too little open space
			for (int j = y; j < y + dims.Y; ++j)
				if (WorldGen.SolidOrSlopedTile(i, j))
					count++;

		if (count > 30)
			goto retry;

		StructureHelper.Generator.GenerateStructure("Structures/HeroMemorial", new Point16(x, y), ModLoader.GetMod("SpiritMod"));

		//Remove non-block tiles from area if there are any
		static void ClimbDownMultitiles(int x, int y)
		{
			Tile tile = Main.tile[x, y];

			if (tile.HasTile && Main.tileFrameImportant[tile.TileType] && tile.TileType != TileID.Plants)
			{
				int offsetY = 0;
				int statue = ModContent.TileType<HerosMemorialStatueTile>();
				Tile checkTile = Main.tile[x, y + offsetY];

				while (checkTile.HasTile && (Main.tileFrameImportant[checkTile.TileType] || TileID.Sets.IsATreeTrunk[checkTile.TileType]) && checkTile.TileType != TileID.Plants && checkTile.TileType != statue)
				{
					offsetY++;
					checkTile = Main.tile[x, y + offsetY];
				}

				WorldGen.KillTile(x, y + offsetY - 1);
			}
		}

		for (int i = 0; i < 9; ++i) //Clears the area
		{
			for (int j = 0; j < 4; ++j)
			{
				ClimbDownMultitiles(x + i, y - j);
				ClimbDownMultitiles(x + i + 13, y - j);
			}
		}
	}

	public static bool NearOtherChests(int x, int y, int width, int height)
	{
		for (int i = x - width; i < x + width; i++)
		{
			for (int j = y - height; j < y + height; j++)
			{
				Tile tileSafely = Framing.GetTileSafely(i, j);

				if (tileSafely.HasTile && TileID.Sets.BasicChest[tileSafely.TileType])
					return true;
			}
		}
		return false;
	}
}
