using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Tiles;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Briar
{
	public class SmallBriarObjects : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = false;
			Main.tileNoFail[Type] = true;
			Main.tileMergeDirt[Type] = true;
            DustType = DustID.Stone;
			HitSound = SoundID.Grass;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.RandomStyleRange = 10;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
			TileObjectData.addTile(Type);

            AddMapEntry(new Color(105, 89, 74));
			TileID.Sets.BreakableWhenPlacing[Type] = true;
		}

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            if (!tileBelow.HasTile || tileBelow.IsHalfBlock || tileBelow.TopSlope)
                WorldGen.KillTile(i, j);

            return true;
        }

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;
	}

	public class SmallBriarObjectsRubble : SmallBriarObjects
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = false;
			Main.tileNoFail[Type] = true;
			Main.tileMergeDirt[Type] = true;
			DustType = DustID.Stone;
			HitSound = SoundID.Grass;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
			TileObjectData.addTile(Type);

			FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<BlastStoneItem>(), Type, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
			RegisterItemDrop(ModContent.ItemType<BlastStoneItem>());
		}
	}
}