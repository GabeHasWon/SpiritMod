using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.SurfaceIce;

public class TundraRock : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 3;
		TileObjectData.addTile(Type);
		TileID.Sets.BreakableWhenPlacing[Type] = true;

		DustType = DustID.RedMoss;

		AddMapEntry(new Color(98, 103, 112));
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
}

public class TundraRockRubble : TundraRock
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		FlexibleTileWand.RubblePlacementSmall.AddVariation(ItemID.StoneBlock, Type, 0);
		RegisterItemDrop(ItemID.StoneBlock);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j) { yield break; }
}