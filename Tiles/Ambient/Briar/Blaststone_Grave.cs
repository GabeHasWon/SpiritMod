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
	public class Blaststone_Grave : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.addTile(Type);
            DustType = DustID.Stone;
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Gravestone");
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

	public class Blaststone_GraveRubble : Blaststone_Grave
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.addTile(Type);
			DustType = DustID.Stone;

			AddMapEntry(new Color(107, 90, 64), Language.GetText($"Mods.SpiritMod.Tiles.{Name}.MapEntry"));

			TileID.Sets.BreakableWhenPlacing[Type] = true;

			FlexibleTileWand.RubblePlacementMedium.AddVariation(ModContent.ItemType<BlastStoneItem>(), Type, 0);
			RegisterItemDrop(ModContent.ItemType<BlastStoneItem>());
		}
	}
}