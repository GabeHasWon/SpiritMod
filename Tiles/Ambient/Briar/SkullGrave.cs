using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
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
			TileObjectData.newTile.Origin = new Point16(0, 3);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 18 };
			TileObjectData.addTile(Type);
            DustType = 7;
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(107, 90, 64), name);
			TileID.Sets.BreakableWhenPlacing[Type] = true;
		}

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
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(0, 3);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 18 };
			TileObjectData.addTile(Type);
			DustType = 7;
			AddMapEntry(new Color(107, 90, 64), Language.GetText($"Mods.SpiritMod.Tiles.{Name}.MapEntry"));
			TileID.Sets.BreakableWhenPlacing[Type] = true;

			FlexibleTileWand.RubblePlacementLarge.AddVariation(ModContent.ItemType<BlastStoneItem>(), Type, 0);
			RegisterItemDrop(ModContent.ItemType<BlastStoneItem>());
		}
	}
}