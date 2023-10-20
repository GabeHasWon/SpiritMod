using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System.Collections.Generic;
using Terraria.GameContent;
using SpiritMod.Items.Sets.FrigidSet;

namespace SpiritMod.Tiles.Ambient.SurfaceIce;

public class IceCube1 : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolidTop[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
		TileObjectData.addTile(Type);
		DustType = DustID.Ice;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.BreakableWhenPlacing[Type] = true;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		if (Main.rand.NextBool(3))
			yield return new Item(ModContent.ItemType<FrigidFragment>());
	}
}

public class IceCube1Rubble : IceCube1
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		FlexibleTileWand.RubblePlacementMedium.AddVariation(ItemID.IceBlock, Type, 0);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ItemID.IceBlock);
	}
}