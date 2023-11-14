using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture
{
	public class TreasureChestTileSmall : ModTile
	{
		public override void SetStaticDefaults()
        {
            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(196, 155, 18), Language.GetText("Mods.SpiritMod.Tiles.TreasureChestTile"));
			DustType = -1;
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			yield return new Item((Framing.GetTileSafely(i, j).TileFrameX > 18) ? ModContent.ItemType<TreasureChestSmall2>() : ModContent.ItemType<TreasureChestSmall>());
		}
	}
}