using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Acid
{
	public class AcidBathtubTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16
			};
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(100, 122, 111), Language.GetText("ItemName.Bathtub"));
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
			TileID.Sets.DisableSmartCursor[Type] = true;
			DustType = -1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) => SoundEngine.PlaySound(SoundID.NPCHit4, new Vector2(i, j) * 16);
	}
}