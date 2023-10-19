using SpiritMod.Items.BossLoot.DuskingDrops;
using SpiritMod.Items.Placeable.Tiles;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Spirit;

public class SpiritRock : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileCut[Type] = false;
		Main.tileNoFail[Type] = true;
		Main.tileMergeDirt[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.RandomStyleRange = 9;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);

		DustType = DustID.PurpleMoss;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;
}

public class SpiritRockRubble : SpiritRock
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<SpiritStoneItem>(), Type, 0, 1, 2, 3, 4, 5, 6, 7, 8);
		RegisterItemDrop(ModContent.ItemType<SpiritStoneItem>());
		TileObjectData.GetTileData(Type, 0).RandomStyleRange = 0;
	}
}