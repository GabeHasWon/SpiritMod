using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System.Collections.Generic;
using SpiritMod.Items.Sets.FrigidSet;
using Terraria.GameContent;
using System.Runtime.CompilerServices;

namespace SpiritMod.Tiles.Ambient.SurfaceIce;

public class IceCube3 : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolidTop[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.addTile(Type);
		DustType = DustID.Ice;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.BreakableWhenPlacing[Type] = true;
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) => SoundEngine.PlaySound(SoundID.Item27);

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		if (Main.rand.NextBool(3))
			yield return new Item(ModContent.ItemType<FrigidFragment>());
	}
}

public class IceCube3Rubble : IceCube3
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		FlexibleTileWand.RubblePlacementSmall.AddVariation(ItemID.IceBlock, Type, 0);
		RegisterItemDrop(ItemID.IceBlock);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j) { yield break; }
}