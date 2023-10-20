using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using SpiritMod.Tiles.Block;
using Terraria.GameContent;

namespace SpiritMod.Tiles.Ambient.Underground
{
	public class RedMushroom2x2 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 2;
			TileObjectData.newTile.AnchorValidTiles = new int[] { TileID.Grass, TileID.Dirt, TileID.Mud, TileID.Stone, TileID.ClayBlock, TileID.ArgonMoss, TileID.BlueMoss, TileID.BrownMoss, 
				TileID.GreenMoss, TileID.KryptonMoss, TileID.LavaMoss, TileID.PurpleMoss, TileID.RedMoss, TileID.XenonMoss, ModContent.TileType<Stargrass>() }; 
			TileObjectData.addTile(Type);
			TileID.Sets.BreakableWhenPlacing[Type] = true;

			DustType = DustID.RedMoss;

			AddMapEntry(new Color(254, 67, 58));
		}
	}

	public class RedMushroom3x2 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
			TileObjectData.newTile.RandomStyleRange = 1;
			TileObjectData.newTile.AnchorValidTiles = new int[] { TileID.Grass, TileID.Dirt, TileID.Mud, TileID.Stone, TileID.ClayBlock, TileID.ArgonMoss, TileID.BlueMoss, TileID.BrownMoss,
				TileID.GreenMoss, TileID.KryptonMoss, TileID.LavaMoss, TileID.PurpleMoss, TileID.RedMoss, TileID.XenonMoss, ModContent.TileType<Stargrass>() };
			TileObjectData.addTile(Type);
			TileID.Sets.BreakableWhenPlacing[Type] = true;

			DustType = DustID.RedMoss;

			AddMapEntry(new Color(254, 67, 58));
		}
	}

	public class RedMushroom1x1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 2;
			TileObjectData.newTile.AnchorValidTiles = new int[] { TileID.Grass, TileID.Dirt, TileID.Mud, TileID.Stone, TileID.ClayBlock, TileID.ArgonMoss, TileID.BlueMoss, TileID.BrownMoss,
				TileID.GreenMoss, TileID.KryptonMoss, TileID.LavaMoss, TileID.PurpleMoss, TileID.RedMoss, TileID.XenonMoss, ModContent.TileType<Stargrass>() };
			TileObjectData.addTile(Type);
			TileID.Sets.BreakableWhenPlacing[Type] = true;

			DustType = DustID.RedMoss;

			AddMapEntry(new Color(254, 67, 58));
		}
	}

	public class RedMushroom2x2Rubble : RedMushroom2x2
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementMedium.AddVariation(ItemID.Mushroom, Type, 0);
			RegisterItemDrop(ItemID.Mushroom);
		}
	}

	public class RedMushroom3x2Rubble : RedMushroom3x2
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.Mushroom, Type, 0);
			RegisterItemDrop(ItemID.Mushroom);
		}
	}

	public class RedMushroom1x1Rubble : RedMushroom1x1
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementSmall.AddVariation(ItemID.Mushroom, Type, 0);
			RegisterItemDrop(ItemID.Mushroom);
		}
	}
}