using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using Terraria.DataStructures;
using SpiritMod.Items.Placeable.Furniture.Sculptures;

namespace SpiritMod.Tiles.Furniture.Sculptures
{
	public class MarbleSculpture2x3 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Marble Sculpture");
			AddMapEntry(new Color(140, 140, 140), name);
			DustType = DustID.Marble;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) 
			=> Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<MarbleSculpture2x3Item>());
	}
}