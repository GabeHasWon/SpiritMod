using Microsoft.Xna.Framework;
using SpiritMod.Items.Sets.HuskstalkSet;
using System.Collections.Generic;
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
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(79, 110, 79));
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
	}

	public class BriarStumpRubble : BriarStump
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementLarge.AddVariation(ModContent.ItemType<AncientBark>(), Type, 0);
			RegisterItemDrop(ModContent.ItemType<AncientBark>());
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield break; }
	}
}