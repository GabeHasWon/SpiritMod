using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using SpiritMod.Items.Consumable.Food;
using System.Collections.Generic;
using Terraria.GameContent;

namespace SpiritMod.Tiles.Ambient.SurfaceIce;

public class SnowBush3 : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolidTop[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
		TileObjectData.addTile(Type);
		DustType = DustID.GrassBlades;
		HitSound = SoundID.Grass;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.BreakableWhenPlacing[Type] = true;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		if (Main.rand.NextBool(7))
			yield return new Item(ModContent.ItemType<IceBerries>());
	}
}

public class SnowBush3Rubble : SnowBush3
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		FlexibleTileWand.RubblePlacementSmall.AddVariation(ItemID.SnowBlock, Type, 0);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ItemID.SnowBlock);
	}
}