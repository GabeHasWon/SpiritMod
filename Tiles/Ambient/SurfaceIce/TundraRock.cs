using Microsoft.Xna.Framework;
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
		TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 3;
		TileObjectData.addTile(Type);
		TileID.Sets.BreakableWhenPlacing[Type] = true;

		DustType = DustID.RedMoss;

		AddMapEntry(new Color(98, 103, 112));
	}
}

public class TundraRockRubble : TundraRock
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);
		TileID.Sets.BreakableWhenPlacing[Type] = true;

		DustType = DustID.RedMoss;

		AddMapEntry(new Color(98, 103, 112));

		FlexibleTileWand.RubblePlacementSmall.AddVariations(ItemID.StoneBlock, Type, 0, 1, 2);
		RegisterItemDrop(ItemID.StoneBlock);
	}
}