using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient
{
	public class GoblinStandardTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			DustType = DustID.Stone;
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			yield return new Item(ItemID.TatteredCloth) { stack = 3 };
		}
	}

	public class GoblinStandardTileRubble : GoblinStandardTile
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.TatteredCloth, Type, 0);
			RegisterItemDrop(ItemID.TatteredCloth);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield break; }
	}
}