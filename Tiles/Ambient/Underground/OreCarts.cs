using Microsoft.Xna.Framework;
using SpiritMod.Items.Consumable.Food;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Underground;

public class OreCarts : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Origin = new Point16(1, 1);
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 8;
		TileObjectData.newTile.AnchorBottom = new AnchorData(Terraria.Enums.AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorAlternateTiles = new int[] { TileID.MinecartTrack };
		TileObjectData.addTile(Type);

		DustType = DustID.WoodFurniture;

		AddMapEntry(new Color(152, 107, 73));
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 12;

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		int frame = Main.tile[i, j].TileFrameX / 54;
		int itemID = frame switch
		{
			0 => ItemID.CopperOre,
			1 => ItemID.IronOre,
			2 => ItemID.SilverOre,
			3 => ItemID.GoldOre,
			4 => ItemID.TinOre,
			5 => ItemID.LeadOre,
			6 => ItemID.TungstenOre,
			7 => ItemID.PlatinumOre,
			_ => ItemID.Amber
		};

		yield return new Item(itemID) { stack = Main.rand.Next(22, 31) };
		yield return new Item(ItemID.Wood) { stack = Main.rand.Next(4, 12) };
	}
}