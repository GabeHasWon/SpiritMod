using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using SpiritMod.Tiles.Ambient;
using Terraria.ID;

namespace SpiritMod.Tiles.Block
{
	public class HalloweenGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMerge[Type][Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileMerge[TileID.Dirt][Type] = true;

			TileID.Sets.Grass[Type] = true;
			TileID.Sets.Conversion.Grass[Type] = true;

			AddMapEntry(new Color(252, 161, 3));
		}

		public override bool CanExplode(int i, int j)
		{
			WorldGen.KillTile(i, j, false, false, true); //Makes the tile completely go away instead of reverting to dirt
			return true;
		}

		public static bool PlaceObject(int x, int y, int type, bool mute = false, int style = 0, int alternate = 0, int random = -1, int direction = -1)
		{
			TileObject toBePlaced;
			if (!TileObject.CanPlace(x, y, type, style, direction, out toBePlaced, false)) {
				return false;
			}
			toBePlaced.random = random;
			if (TileObject.Place(toBePlaced) && !mute) {
				WorldGen.SquareTileFrame(x, y, true);
				//   Main.PlaySound(SoundID.Dig, x * 16, y * 16, 1, 1f, 0f);
			}
			return false;
		}

		public override void RandomUpdate(int i, int j)
		{
			if (!Framing.GetTileSafely(i, j - 1).HasTile && Main.rand.NextBool(20)) {
				int style = Main.rand.Next(23);
				if (PlaceObject(i, j - 1, ModContent.TileType<SpookyFoliage>(), false, style))
					NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<SpookyFoliage>(), style, 0, -1, -1);
			}

			if (SpreadHelper.Spread(i, j, Type, 4, TileID.Dirt) && Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None);
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.028f;
			g = 0.153f;
			b = 0.081f;
		}
	}
}

