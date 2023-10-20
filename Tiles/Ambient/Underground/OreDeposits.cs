using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Underground
{
	public class OreDeposits : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 4;
			TileObjectData.addTile(Type);

			DustType = DustID.Stone;

			AddMapEntry(new Color(165, 165, 165));
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			int itemID = ItemID.TungstenOre;
			int frame = Main.tile[i, j].TileFrameX / 54;

			if (frame == 1)
				itemID = ItemID.SilverOre;
			else if (frame == 2)
				itemID = ItemID.IronOre;
			else if (frame == 3)
				itemID = ItemID.GoldOre;

			yield return new Item(itemID, Main.rand.Next(22, 31));
		}
	}

	public class OreDepositsRubble : OreDeposits
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			DustType = DustID.Stone;

			AddMapEntry(new Color(165, 165, 165));

			FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.TungstenOre, Type, 0);
			FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.SilverOre, Type, 1);
			FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.IronOre, Type, 2);
			FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.GoldOre, Type, 3);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			int itemID = ItemID.TungstenOre;
			int frame = Main.tile[i, j].TileFrameX / 54;

			if (frame == 1)
				itemID = ItemID.SilverOre;
			else if (frame == 2)
				itemID = ItemID.IronOre;
			else if (frame == 3)
				itemID = ItemID.GoldOre;

			yield return new Item(itemID);
		}
	}
}