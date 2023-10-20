using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Briar
{
	public class SkullGrave : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16
			};
			TileObjectData.addTile(Type);
            DustType = 7;
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Gravestone");
			AddMapEntry(new Color(107, 90, 64), name);
			TileID.Sets.BreakableWhenPlacing[Type] = true;
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
		
		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 2);
            if (!tileBelow.HasTile || tileBelow.IsHalfBlock || tileBelow.TopSlope)
                WorldGen.KillTile(i, j);

            return true;
        }
    }

	public class SkullGraveRubble : SkullGrave
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			FlexibleTileWand.RubblePlacementLarge.AddVariation(ModContent.ItemType<BlastStoneItem>(), Type, 0);
			RegisterItemDrop(ModContent.ItemType<BlastStoneItem>());
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) { yield break; }
	}
}