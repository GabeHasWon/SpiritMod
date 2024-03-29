using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture.MarblePillars;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Pillars
{
	public class Pillar7Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolidTop[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Origin = new Point16(0, 3);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 2, 0);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16
			};
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Pillar");
			AddMapEntry(new Color(179, 174, 161), name);
			DustType = -1;
			TileID.Sets.DisableSmartCursor[Type] = true;
		}
	}
}