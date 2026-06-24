using Microsoft.Xna.Framework;
using SpiritMod.Tiles.Ambient.Spirit;
using SpiritMod.Tiles.Block;
using SpiritMod.Tiles.Walls.Natural;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.World;

/// <summary> Generates the Spirit biome in the world using <see cref="SpawnSpiritBiome"/>. </summary>
internal static class SpiritGeneration
{
	internal static void SpawnSpiritBiome()
	{
		int firstX = WorldGen.genRand.Next(100, (Main.maxTilesX / 2) - 500);
		if (Main.dungeonX > Main.maxTilesX / 2) //rightside dungeon
			firstX = WorldGen.genRand.Next((Main.maxTilesX / 2) + 300, Main.maxTilesX - 500);

		int x = firstX;
		int y = 0;

		int xAxisMid = x + 70;
		int xAxisEdge = x + 380;

		int distanceFromCenter = 0;

		for (int i = 0; i < Main.maxTilesY; i++)
		{
			y++;
			x = firstX;

			for (int j = 0; j < 450; j++)
			{
				x++;

				if (WorldGen.InWorld(x, y, 30))
				{
					if (x < xAxisMid - 1)
						distanceFromCenter = xAxisMid - x;
					else if (x > xAxisEdge + 1)
						distanceFromCenter = x - xAxisEdge;

					if (Main.rand.Next(distanceFromCenter) < 18)
					{
						DoConversion(x, y);

						if (Main.netMode != NetmodeID.SinglePlayer)
							NetMessage.SendTileSquare(-1, x, y);
					}

					if (Main.tile[x, y].TileType == ModContent.TileType<SpiritStone>() && y > (int)((Main.rockLayer + Main.maxTilesY - 500) / 2f) && Main.rand.NextBool(300))
						WorldGen.OreRunner(x, y, WorldGen.genRand.Next(5, 7), 1, (ushort)ModContent.TileType<Items.Sets.SpiritSet.SpiritOreTile>()); //Adds ore
				}
			}
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			string message = Language.GetTextValue("Mods.SpiritMod.Misc.SpiritSpread");
			MyWorld.spiritBiome = true;

			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(message, Color.Orange);
			else
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), Color.Orange);
		}
	}

	private static void DoConversion(int i, int j)
	{
		var tile = Main.tile[i, j];
		int tileType = tile.TileType;
		int wallType = tile.WallType;

		int[] invalidDecor = [TileID.CorruptThorns, TileID.Vines, TileID.JungleVines, TileID.HallowedVines, TileID.Stalactite, TileID.LargePiles2, TileID.CrimsonVines];
		int[] plants = [TileID.Plants, TileID.CorruptPlants, TileID.CrimsonPlants, TileID.HallowedPlants];
		int[] tallPlants = [TileID.Plants2, TileID.HallowedPlants2];

		if (tile.HasTile)
		{
			if (TileID.Sets.Conversion.Grass[tileType]) //Convert tiles
				tileType = ModContent.TileType<SpiritGrass>();
			else if (TileID.Sets.Conversion.Stone[tileType])
				tileType = ModContent.TileType<SpiritStone>();
			else if (TileID.Sets.Conversion.Ice[tileType])
				tileType = ModContent.TileType<SpiritIce>();
			else if (TileID.Sets.Conversion.Sand[tileType])
				tileType = ModContent.TileType<Spiritsand>();
			else if (tileType == TileID.Dirt)
				tileType = ModContent.TileType<SpiritDirt>();
			else if (plants.Contains(tileType)) //Frame importants
				tileType = ModContent.TileType<SpiritFoliage>();
			else if (tallPlants.Contains(tileType))
				tileType = ModContent.TileType<SpiritTallgrass>();
			else if (tileType == TileID.SmallPiles)
			{
				if (tile.TileFrameY > 0)
					tileType = ModContent.TileType<SpiritRockMedium>();
				else
					tileType = ModContent.TileType<SpiritRock>();
			}
			else if (invalidDecor.Contains(tile.TileType))
				tile.HasTile = false; //Remove the invalid decor tile

			//savanna overlap
			if (ModLoader.TryGetMod("SpiritReforged", out Mod mod))
			{
				//for specifically hardened sand in the savanna, since it shouldn't ever touch the main desert bc it's on dungeon side
				if (tileType == TileID.HardenedSand || tileType == TileID.CorruptHardenedSand || tileType == TileID.CrimsonHardenedSand || tileType == TileID.HallowHardenedSand)
				{
					tileType = ModContent.TileType<SpiritDirt>();
				}
				if (mod.TryFind("SavannaDirt", out ModTile coarseDirt))
				{
					if (tileType == coarseDirt.Type)
						tileType = ModContent.TileType<SpiritDirt>();
				}
				if (mod.TryFind("SavannaGrass", out ModTile grass) && mod.TryFind("SavannaGrassCorrupt", out ModTile corruptGrass) && mod.TryFind("SavannaGrassCrimson", out ModTile crimsonGrass) && mod.TryFind("SavannaGrassHallow", out ModTile hallowGrass))
				{
					if (tileType == grass.Type || tileType == corruptGrass.Type || tileType == crimsonGrass.Type || tileType == hallowGrass.Type)
						tileType = ModContent.TileType<SpiritGrass>();
				}
				if (mod.TryFind("SavannaRockSmall", out ModTile savannaRocksSmall))
				{
					if (tileType == savannaRocksSmall.Type)
						tileType = ModContent.TileType<SpiritRock>();
				}
				if (mod.TryFind("SavannaShrubs", out ModTile shrubs) && mod.TryFind("SavannaShrubsCorrupt", out ModTile corruptShrubs) && mod.TryFind("SavannaShrubsCrimson", out ModTile crimsonShrubs)&& mod.TryFind("SavannaShrubsHallow", out ModTile hallowShrubs))
				{
					if (tileType == shrubs.Type || tileType == corruptShrubs.Type || tileType == crimsonShrubs.Type || tileType == hallowShrubs.Type)
						tileType = ModContent.TileType<SpiritRockMedium>();
				}
				if (mod.TryFind("ElephantGrass", out ModTile elephantGrass) && mod.TryFind("ElephantGrassCorrupt", out ModTile corruptElephantGrass) && mod.TryFind("ElephantGrassCrimson", out ModTile crimsonElephantGrass) && mod.TryFind("ElephantGrassHallow", out ModTile hallowElephantGrass))
				{
					if (tileType == grass.Type || tileType == corruptElephantGrass.Type || tileType == crimsonElephantGrass.Type || tileType == hallowElephantGrass.Type)
						tileType = ModContent.TileType<SpiritTallgrass>();
				}
				if (mod.TryFind("SavannaFoliage", out ModTile foliage) && mod.TryFind("SavannaFoliageCorrupt", out ModTile corruptFoliage) && mod.TryFind("SavannaFoliageCrimson", out ModTile crimsonFoliage) && mod.TryFind("SavannaFoliageHallow", out ModTile hallowFoliage))
				{
					if (tileType == foliage.Type || tileType == corruptFoliage.Type || tileType == crimsonFoliage.Type || tileType == hallowFoliage.Type)
						tileType = ModContent.TileType<SpiritFoliage>();
				}
				if (mod.TryFind("TermiteMound", out ModTile termite) && mod.TryFind("SavannaRockLarge", out ModTile bigRock) && mod.TryFind("SavannaVine", out ModTile vine) && mod.TryFind("SavannaVineCorrupt", out ModTile corruptVine) && mod.TryFind("SavannaVineCrimson", out ModTile crimsonVine) && mod.TryFind("SavannaVineHallow", out ModTile hallowVine))
				{
					if (tileType == termite.Type || tileType == bigRock.Type || tileType == vine.Type || tileType == corruptVine.Type || tileType == crimsonVine.Type || tileType == hallowVine.Type)
						tile.HasTile = false;
				}
			}

		}

		if (WallID.Sets.Conversion.Grass[wallType]) //Convert walls
			wallType = ModContent.WallType<SpiritWallNatural>();

		tile.TileType = (ushort)tileType;
		tile.WallType = (ushort)wallType;
	}
}