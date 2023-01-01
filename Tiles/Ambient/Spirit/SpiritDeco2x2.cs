using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Systems;
using SpiritMod.Tiles.Block;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Spirit
{
	public class SpiritDeco2x2 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = false;
			Main.tileNoFail[Type] = true;
			Main.tileMergeDirt[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<SpiritGrass>(), ModContent.TileType<SpiritDirt>(), ModContent.TileType<SpiritStone>() };
			TileObjectData.newTile.RandomStyleRange = 8;
			TileObjectData.addTile(Type);

			TileID.Sets.DisableSmartCursor[Type] = true;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.07f, 0.07f, 0.25f);

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			int frame = Main.tile[i, j].TileFrameX / 36;

			if (frame >= 5)
				TileSwaySystem.DrawGrassSway(spriteBatch, TextureAssets.Tile[Type].Value, i, j, Lighting.GetColor(i, j));
			return false;
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			int frame = tile.TileFrameX / 36;

			if (tile.TileFrameX % 36 == 0 && tile.TileFrameY == 0 && Main.netMode != NetmodeID.Server)
				SoundEngine.PlaySound((frame >= 5) ? SoundID.Dig : SoundID.Grass, new Vector2(i, j) * 16);
			for (int o = 0; o < 2; o++)
				Dust.NewDust(new Vector2(i, j) * 16, 16, 16, (frame >= 5) ? DustID.PurpleMoss : DustID.UnusedWhiteBluePurple);
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) => spriteEffects = (i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
	}
}