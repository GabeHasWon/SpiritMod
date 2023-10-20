using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using SpiritMod.Tiles.Block;
using Terraria.GameContent;

namespace SpiritMod.Tiles.Ambient.Underground
{
	public class BrownMushrooms : ModTile
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

			DustType = DustID.BrownMoss;

			AddMapEntry(new Color(139, 81, 68));
		}
	}

	public class BrownMushroomsRubble : BrownMushrooms
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementMedium.AddVariation(ItemID.Mushroom, Type, 0);
			RegisterItemDrop(ItemID.Mushroom);
		}
	}

	public class BrownMushroomLarge : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
			TileObjectData.newTile.RandomStyleRange = 1;
			TileObjectData.newTile.AnchorValidTiles = new int[] { TileID.Grass, TileID.Dirt, TileID.Mud, TileID.Stone, TileID.ClayBlock, TileID.ArgonMoss, TileID.BlueMoss, TileID.BrownMoss,
				TileID.GreenMoss, TileID.KryptonMoss, TileID.LavaMoss, TileID.PurpleMoss, TileID.RedMoss, TileID.XenonMoss, ModContent.TileType<Stargrass>() };
			TileObjectData.addTile(Type);
			TileID.Sets.BreakableWhenPlacing[Type] = true;

			DustType = DustID.BrownMoss;

			AddMapEntry(new Color(139, 81, 68));
		}
	}

	public class BrownMushroomLargeRubble : BrownMushroomLarge
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.Mushroom, Type, 0);
			RegisterItemDrop(ItemID.Mushroom);
		}
	}
}