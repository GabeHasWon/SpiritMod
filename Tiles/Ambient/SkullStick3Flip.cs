using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient
{
	public class SkullStick3Flip : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
			};
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Skull Stick");
			AddMapEntry(new Color(107, 90, 64), name);
			AdjTiles = new int[] { TileID.Lamps };
			TileID.Sets.BreakableWhenPlacing[Type] = true;
		}
	}

	public class SkullStick3FlipRubble : SkullStick3Flip
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementMedium.AddVariation(ItemID.Bone, Type, 0);
			RegisterItemDrop(ItemID.Bone);
		}
	}
}