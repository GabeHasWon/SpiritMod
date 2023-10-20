using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Piles
{
	public class CopperPile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(70, 70, 70));
			DustType = DustID.CopperCoin;
			TileID.Sets.DisableSmartCursor[Type] = true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.CopperOre, 8); }
	}

	public class CopperPileRubble : CopperPile
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementSmall.AddVariation(ItemID.CopperOre, Type, 0);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.CopperOre); }
	}

	public class TinPile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 1;

			AddMapEntry(new Color(70, 70, 70));
			TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
			DustType = DustID.SilverCoin;
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.TinOre, 8); }
	}

	public class TinPileRubble : TinPile
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementSmall.AddVariation(ItemID.TinOre, Type, 0);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.TinOre); }
	}

	public class IronPile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 2;

			AddMapEntry(new Color(70, 70, 70));
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.addTile(Type);
			DustType = DustID.SilverCoin;
		}
		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.IronOre, 11); }
	}

	public class IronPileRubble : IronPile
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementSmall.AddVariation(ItemID.IronOre, Type, 0);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.IronOre); }
	}

	public class LeadPile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 2;

			AddMapEntry(new Color(70, 70, 70));
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.addTile(Type);
			DustType = DustID.SilverCoin;
		}
		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.LeadOre, 11); }
	}

	public class LeadPileRubble : LeadPile
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementSmall.AddVariation(ItemID.LeadOre, Type, 0);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.LeadOre); }
	}

	public class SilverPile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 2;

			AddMapEntry(new Color(70, 70, 70));
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.addTile(Type);
			DustType = DustID.SilverCoin;
		}
		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.SilverOre, 12); }
	}

	public class SilverPileRubble : SilverPile
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementMedium.AddVariation(ItemID.SilverOre, Type, 0);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.SilverOre); }
	}

	public class TungstenPile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 2;

			AddMapEntry(new Color(70, 70, 70));
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.addTile(Type);
			DustType = DustID.SilverCoin;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.TungstenOre, 12); }
	}

	public class TungstenPileRubble : TungstenPile
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementMedium.AddVariation(ItemID.TungstenOre, Type, 0);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.TungstenOre); }
	}

	public class GoldPile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 2;

			AddMapEntry(new Color(70, 70, 70));
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.addTile(Type);
			DustType = DustID.GoldCoin;
		}
		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.GoldOre, 15); }
	}

	public class GoldPileRubble : GoldPile
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.GoldOre, Type, 0);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.GoldOre); }
	}

	public class PlatinumPile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 2;

			AddMapEntry(new Color(70, 70, 70));
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.addTile(Type);
			DustType = DustID.SilverCoin;
		}
		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.PlatinumOre, 15); }
	}

	public class PlatinumPileRubble : PlatinumPile
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.PlatinumOre, Type, 0);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.PlatinumOre); }
	}
}