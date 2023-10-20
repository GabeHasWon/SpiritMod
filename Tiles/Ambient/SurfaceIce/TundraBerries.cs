using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System.Collections.Generic;
using Terraria.GameContent;

namespace SpiritMod.Tiles.Ambient.SurfaceIce;

public class TundraBerries2x2 : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolidTop[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
		TileObjectData.addTile(Type);

		DustType = DustID.BorealWood;
		HitSound = SoundID.Dig;

		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.BreakableWhenPlacing[Type] = true;
	}
}

public class TundraBerries2x2Rubble : TundraBerries2x2
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		FlexibleTileWand.RubblePlacementMedium.AddVariation(ItemID.BorealWood, Type, 0);
		RegisterItemDrop(ItemID.BorealWood);
	}
}

public class TundraBerries1x2 : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolidTop[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
		TileObjectData.newTile.RandomStyleRange = 2;
		TileObjectData.newTile.StyleHorizontal = true;

		TileObjectData.addTile(Type);

		DustType = DustID.BorealWood;
		HitSound = SoundID.Dig;

		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.BreakableWhenPlacing[Type] = true;
	}
}

public class TundraBerries1x2Rubble : TundraBerries1x2
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		FlexibleTileWand.RubblePlacementMedium.AddVariation(ItemID.BorealWood, Type, 0);
		RegisterItemDrop(ItemID.BorealWood);
	}
}