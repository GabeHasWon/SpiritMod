using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Systems;
using SpiritMod.Tiles.Block;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Spirit
{
	public class SpiritTallgrass : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLavaDeath[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileSolidTop[Type] = false;
			Main.tileSolid[Type] = false;
			Main.tileCut[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<SpiritGrass>() };
			TileObjectData.newTile.RandomStyleRange = 13;
			TileObjectData.addTile(Type);

			TileID.Sets.DisableSmartCursor[Type] = true;
			DustType = DustID.UnusedWhiteBluePurple;
			HitSound = SoundID.Grass;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			int frame = Main.tile[i, j].TileFrameX / 18;
			if (frame >= 6)
				(r, g, b) = (0.07f, 0.07f, 0.25f);
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			TileSwaySystem.DrawGrassSway(spriteBatch, TextureAssets.Tile[Type].Value, i, j, Lighting.GetColor(i, j));
			return false;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) => TileSwaySystem.DrawGrassSway(spriteBatch, Texture + "_Glow", i, j, new Color(180, 180, 180, 100));

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) => spriteEffects = (i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
	}
}