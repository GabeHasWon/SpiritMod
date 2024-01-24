using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Sets.FloranSet
{
	public class FloranBarTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.addTile(Type);
			AdjTiles = new int[] { TileID.MetalBars };
			DustType = DustID.JunglePlants;
			AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.Bar"));
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 3 : 10;
	}
}