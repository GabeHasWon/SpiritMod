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

namespace SpiritMod.World
{
	internal static class SpiritGeneration
	{
		internal static void SpawnSpiritBiome()
		{
			int firstX = WorldGen.genRand.Next(100, (Main.maxTilesX / 2) - 500);
			if (Main.dungeonX > Main.maxTilesX / 2) //rightside dungeon
				firstX = WorldGen.genRand.Next((Main.maxTilesX / 2) + 300, Main.maxTilesX - 500);

			int xAxis = firstX;
			int xAxisMid = xAxis + 70;
			int xAxisEdge = xAxis + 380;
			int yAxis = 0;

			int distanceFromCenter = 0;

			int[] Grasses = { TileID.Grass, TileID.CorruptGrass, TileID.HallowedGrass, TileID.CrimsonGrass };
			int[] Ices = { TileID.IceBlock, TileID.CorruptIce, TileID.HallowedIce, TileID.FleshIce };
			int[] Stones = { TileID.Stone, TileID.Ebonstone, TileID.Pearlstone, TileID.Crimstone, TileID.GreenMoss, TileID.BrownMoss, TileID.RedMoss, TileID.BlueMoss, TileID.PurpleMoss };
			int[] Sands = { TileID.Sand, TileID.Ebonsand, TileID.Pearlsand, TileID.Crimsand };

			int[] Decors = { TileID.CorruptThorns, TileID.Vines, TileID.JungleVines, TileID.HallowedVines, TileID.Stalactite, TileID.LargePiles2, TileID.CrimsonVines };
			int[] Plants = { TileID.Plants, TileID.CorruptPlants, TileID.CrimsonPlants, TileID.HallowedPlants };
			int[] TallPlants = { TileID.Plants2, TileID.HallowedPlants2 };

			for (int y = 0; y < Main.maxTilesY; y++)
			{
				yAxis++;
				xAxis = firstX;

				for (int i = 0; i < 450; i++)
				{
					xAxis++;

					if (WorldGen.InWorld(xAxis, yAxis, 30) && Framing.GetTileSafely(xAxis, yAxis).HasTile)
					{
						int type = -1;

						int requiredDist = 10;

						#region set types
						//Blocks
						if (Main.tile[xAxis, yAxis].TileType == TileID.Dirt)
							type = ModContent.TileType<SpiritDirt>();
						if (Grasses.Contains(Main.tile[xAxis, yAxis].TileType))
							type = ModContent.TileType<SpiritGrass>();
						else if (Ices.Contains(Main.tile[xAxis, yAxis].TileType))
							type = ModContent.TileType<SpiritIce>();
						else if (Stones.Contains(Main.tile[xAxis, yAxis].TileType))
							type = ModContent.TileType<SpiritStone>();
						else if (Sands.Contains(Main.tile[xAxis, yAxis].TileType))
							type = ModContent.TileType<Spiritsand>();

						if (type == -1)
							requiredDist = 18;

						if (Plants.Contains(Main.tile[xAxis, yAxis].TileType))
							type = ModContent.TileType<SpiritFoliage>();
						else if (TallPlants.Contains(Main.tile[xAxis, yAxis].TileType))
							type = ModContent.TileType<SpiritTallgrass>();
						else if (Main.tile[xAxis, yAxis].TileType == TileID.SmallPiles)
						{
							if (Main.tile[xAxis, yAxis].TileFrameY > 0)
								type = ModContent.TileType<SpiritRockMedium>();
							else
								type = ModContent.TileType<SpiritRock>();
						}
						else if (Main.tile[xAxis, yAxis].TileType == TileID.LargePiles)
							type = ModContent.TileType<SpiritDeco2x2>();
						#endregion

						if (xAxis < xAxisMid - 1)
							distanceFromCenter = xAxisMid - xAxis;
						else if (xAxis > xAxisEdge + 1)
							distanceFromCenter = xAxis - xAxisEdge;

						if (type != -1 && Main.rand.Next(distanceFromCenter) < requiredDist)
							Main.tile[xAxis, yAxis].TileType = (ushort)type; //Converts tiles

						if (WallID.Sets.Conversion.Grass[Main.tile[xAxis, yAxis].WallType] && Main.rand.Next(distanceFromCenter) < 18)
							Main.tile[xAxis, yAxis].WallType = (ushort)ModContent.WallType<SpiritWallNatural>(); //Converts walls

						if (Decors.Contains(Main.tile[xAxis, yAxis].TileType) && Main.rand.Next(distanceFromCenter) < 18)
						{
							Tile tile = Main.tile[xAxis, yAxis];
							tile.HasTile = false; //Removes remaining decor
						}

						if (Main.tile[xAxis, yAxis].TileType == ModContent.TileType<SpiritStone>() && yAxis > (int)((Main.rockLayer + Main.maxTilesY - 500) / 2f) && Main.rand.NextBool(300))
							WorldGen.TileRunner(xAxis, yAxis, WorldGen.genRand.Next(5, 7), 1, ModContent.TileType<Items.Sets.SpiritSet.SpiritOreTile>(), false, 0f, 0f, true, true); //Adds ore
					}
				}
			}

			string message = "The Spirits spread through the Land...";

			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(message, Color.Orange);
			else if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.WorldData);
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), Color.Orange, -1);
			}
		}
	}
}