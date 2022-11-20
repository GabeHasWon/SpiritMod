using SpiritMod.Tiles.Ambient;
using SpiritMod.Tiles.Block;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Items
{
	internal class MiscItemHooks : ILoadable
	{
		private int[] FlowerBootsPlantSubIds = new int[0];

		public void Load(Mod mod) => On.Terraria.Player.DoBootsEffect_PlaceFlowersOnTile += Player_DoBootsEffect_PlaceFlowersOnTile;

		/// <summary>
		/// Stargrass hook detour for Flower Boots.
		/// </summary>
		private bool Player_DoBootsEffect_PlaceFlowersOnTile(On.Terraria.Player.orig_DoBootsEffect_PlaceFlowersOnTile orig, Player self, int X, int Y)
		{
			Tile tile = Main.tile[X, Y];
			if (!tile.HasTile && tile.LiquidAmount == 0 && WorldGen.SolidTile(X, Y + 1))
			{
				tile.TileFrameY = 0;
				tile.Slope = SlopeType.Solid;
				tile.IsHalfBlock = false;

				if (Main.tile[X, Y + 1].TileType == ModContent.TileType<Stargrass>())
				{
					if (Main.rand.NextBool(2))
					{
						if (FlowerBootsPlantSubIds.Length == 0) //Just a lot of sub ids (frames) for the plants
						{
							var list = new List<int> { 6, 7 };
							for (int i = 9; i < 45; ++i)
								list.Add(i);

							FlowerBootsPlantSubIds = list.ToArray();
						}

						tile.HasTile = true;
						tile.TileType = TileID.Plants;
						tile.TileFrameX = (short)(Main.rand.Next(FlowerBootsPlantSubIds) * 18);
						tile.TileColor = Main.tile[X, Y + 1].TileColor;
					}
					else
						WorldGen.PlaceTile(X, Y, ModContent.TileType<StargrassFlowers>(), true, true, -1, Main.rand.Next(12));

					if (Main.netMode == NetmodeID.MultiplayerClient)
						NetMessage.SendTileSquare(-1, X, Y);
					return true;
				}
			}

			return orig(self, X, Y);
		}

		public void Unload() { }
	}
}
