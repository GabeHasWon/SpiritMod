using SpiritMod.Tiles.Ambient.Kelp;
using SpiritMod.Tiles.Block;
using SpiritMod.Items.Sets.FloatingItems.Driftwood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace SpiritMod.Tiles.Ambient.Ocean;

public class OceanGlobalTile : GlobalTile
{
	public override void RandomUpdate(int i, int j, int type)
	{
		HashSet<int> sands = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand, ModContent.TileType<Spiritsand>()]; //All valid sands
		HashSet<int> woods = [TileID.WoodBlock, TileID.BorealWood, TileID.Ebonwood, TileID.DynastyWood, TileID.RichMahogany, TileID.PalmWood, TileID.Shadewood, TileID.WoodenBeam,
			ModContent.TileType<SpiritWood>(), ModContent.TileType<LivingBriarWood>(), ModContent.TileType<DriftwoodTile>(), TileID.Pearlwood ];

		bool inOcean = (i < Main.maxTilesX / 16 || i > Main.maxTilesX / 16 * 15) && j < (int)Main.worldSurface; //Might need adjustment; don't know if this will be exclusively in the ocean
		bool inWorldBounds = i > 40 && i < Main.maxTilesX - 40;

		if (sands.Contains(type) && inOcean && inWorldBounds && !Framing.GetTileSafely(i, j - 1).HasTile && WorldGen.SolidTile(i, j) && Main.rand.NextBool(3)) //woo
		{
			if (Framing.GetTileSafely(i, j - 1).LiquidAmount > 200) //water stuff
			{
				if (Main.rand.NextBool(80))
				{
					WorldGen.PlaceTile(i, j - 1, ModContent.TileType<OceanKelp>()); //Kelp spawning
					SyncSpotIfNecessary(i, j - 1);
				}

				bool openSpace = !Framing.GetTileSafely(i, j - 2).HasTile;
				if (openSpace && Main.rand.NextBool(40)) //1x2 kelp
				{
					WorldGen.PlaceObject(i, j - 1, ModContent.TileType<Kelp1x2>());
					SyncSpotIfNecessary(i, j - 2, 1, 2);
				}

				openSpace = !Framing.GetTileSafely(i + 1, j - 1).HasTile && !Framing.GetTileSafely(i + 1, j - 2).HasTile && !Framing.GetTileSafely(i, j - 2).HasTile;
				if (openSpace && WorldGen.SolidTile(i + 1, j) && Main.rand.NextBool(80)) //2x2 kelp
				{
					WorldGen.PlaceObject(i, j - 1, ModContent.TileType<Kelp2x2>());
					SyncSpotIfNecessary(i, j - 2, 2, 2);
				}

				openSpace = !Framing.GetTileSafely(i + 1, j - 1).HasTile && !Framing.GetTileSafely(i + 1, j - 2).HasTile && !Framing.GetTileSafely(i, j - 2).HasTile && !Framing.GetTileSafely(i + 1, j - 3).HasTile && !Framing.GetTileSafely(i, j - 3).HasTile;
				if (openSpace && WorldGen.SolidTile(i + 1, j) && Main.rand.NextBool(90)) //2x3 kelp
				{
					WorldGen.PlaceObject(i, j - 1, ModContent.TileType<Kelp2x3>());
					SyncSpotIfNecessary(i, j - 3, 2, 3);
				}
			}
			else
			{
				if (Main.rand.NextBool(12))
				{
					WorldGen.PlaceTile(i, j - 1, ModContent.TileType<Seagrass>(), true, true, -1, Main.rand.Next(16));
					SyncSpotIfNecessary(i, j - 1);
				}
			}

			for (int k = i - 1; k < i + 2; ++k)
			{
				for (int l = j - 1; l < j + 2; ++l)
				{
					if (k == i && l == j) 
						continue; //Dont check myself

					Tile cur = Framing.GetTileSafely(k, l);
					if (!cur.HasTile && woods.Contains(cur.TileType) && cur.LiquidAmount > 155 && cur.LiquidType == LiquidID.Water && Main.rand.NextBool(6))
						WorldGen.PlaceTile(k, l, ModContent.TileType<Mussel>());
				}
			}
		}
	}

	public static void SyncSpotIfNecessary(int i, int j, int width = 1, int height = 1)
	{
		if (Main.netMode == NetmodeID.Server)
			NetMessage.SendTileSquare(-1, i, j, width, height, TileChangeType.None);
	}
}
