using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Vanilla;

internal class PearlstoneRandomUpdate : RandomUpdate
{
	public override void OnTick(int i, int j, int type)
	{
		if (type is TileID.Pearlstone)
		{
			bool inLavaLayer = j > (int)Main.rockLayer && j < Main.maxTilesY - 250;

			if (!inLavaLayer)
				return;

			Tile tile = Framing.GetTileSafely(i, j);
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);

			if (WorldGen.genRand.NextBool(20) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
			{
				if (!tile.BottomSlope)
				{
					tileBelow.TileType = (ushort)ModContent.TileType<Ambient.HangingChimes.HangingChimes>();
					tileBelow.HasTile = true;
					WorldGen.SquareTileFrame(i, j + 1, true);
					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
				}
			}
		}
	}
}
