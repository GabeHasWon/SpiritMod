using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Spirit
{
	public class SpiritRock : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = false;
			Main.tileNoFail[Type] = true;
			Main.tileMergeDirt[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.RandomStyleRange = 9;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			DustType = DustID.PurpleMoss;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;
	}
}