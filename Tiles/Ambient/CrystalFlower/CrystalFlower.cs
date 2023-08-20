using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.AccessoriesMisc.CrystalFlower;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.CrystalFlower
{
	public class CrystalFlower : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.newTile.AnchorAlternateTiles = new int[] { TileID.Cactus };
			TileObjectData.newTile.AnchorBottom = new Terraria.DataStructures.AnchorData(Terraria.Enums.AnchorType.AlternateTile, 1, 0);
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(9, 170, 219), name);

			MineResist = 1.2f;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
				zero = Vector2.Zero;

			spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Ambient/CrystalFlower/CrystalFlower_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			if (Main.rand.NextBool(20))
				Dust.NewDustPerfect((new Vector2(i, j) * 16) + new Vector2(16 * Main.rand.NextFloat(), 16 * Main.rand.NextFloat()), DustID.CrystalSerpent_Pink, Vector2.Zero, 0, default, Main.rand.NextFloat(0.5f, 1.0f)).noGravity = true;
		}
	}
}