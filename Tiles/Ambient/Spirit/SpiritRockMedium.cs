using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Spirit
{
	public class SpiritRockMedium : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = false;
			Main.tileNoFail[Type] = true;
			Main.tileMergeDirt[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.RandomStyleRange = 6;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			DustType = DustID.PurpleMoss;
			HitSound = SoundID.Tink;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 4;
	}
}