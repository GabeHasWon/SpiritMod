using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Underground
{
	public class LargeRock : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(1, 2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
			TileObjectData.newTile.StyleHorizontal = false;
			TileObjectData.newTile.RandomStyleRange = 1;
			TileObjectData.addTile(Type);
			TileID.Sets.BreakableWhenPlacing[Type] = true;

			DustType = DustID.Stone;

			AddMapEntry(new Color(165, 165, 165));
		}
	}
}