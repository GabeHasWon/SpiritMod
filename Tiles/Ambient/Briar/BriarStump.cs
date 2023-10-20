using Microsoft.Xna.Framework;
using SpiritMod.Items.Sets.HuskstalkSet;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Briar
{
	public class BriarStump : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileLighted[Type] = true;
			DustType = 7;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(79, 110, 79));
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;
	}

	public class BriarStumpRubble : BriarStump
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<AncientBark>(), Type, 0, 1);
			RegisterItemDrop(ModContent.ItemType<AncientBark>());
		}
	}
}