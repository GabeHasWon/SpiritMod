using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Spirit
{
	public class SpiritFoliage : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileLighted[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.WaterDeath = false;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
			TileObjectData.newTile.DrawYOffset = -2;
			TileObjectData.newTile.Style = 0;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;

			for (int i = 0; i < 15; i++)
			{
				TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
				TileObjectData.addSubTile(TileObjectData.newSubTile.Style);
			}
			TileObjectData.addTile(Type);

			TileID.Sets.SwaysInWindBasic[Type] = true;

			DustType = DustID.UnusedWhiteBluePurple;
			HitSound = SoundID.Grass;

			AddMapEntry(new Color(50, 90, 145));
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.07f, 0.07f, 0.25f);
	}
}