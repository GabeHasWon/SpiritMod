using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace SpiritMod.Tiles.Furniture
{
	public class ForgottenKingStatue : ModTile
	{
		public override void SetStaticDefaults()
        {
            Main.tileTable[Type] = true;
            Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Forgotten King Statue");
			AddMapEntry(new Color(140, 140, 140), name);

			DustType = DustID.Stone;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new Terraria.DataStructures.EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<ForgottenKingStatueItem>());
	}
}