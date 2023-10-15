using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient
{
	public class UnstableIcicle : ModTile
	{
		public const int NumStyles = 12;

		public virtual int TileVariant => 0;

		public override string Texture => "SpiritMod/Tiles/Ambient/UnstableIcicle";

		public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = false;
			Main.tileCut[Type] = true;
			Main.tileNoFail[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
			TileObjectData.newTile.AnchorValidTiles = new int[] { TileID.IceBlock, TileID.SnowBlock };
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(100, 100, 60));
			DustType = -1;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			if (Main.rand.NextBool(35))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(i, j) * 16, 16, 16, DustID.TreasureSparkle, 0, 0, 100, default, .75f);
				dust.velocity = Vector2.Zero;
				dust.noLightEmittence = true;
			}
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			if (tile.TileFrameY == 0 && !fail)
			{
				Projectile proj = Projectile.NewProjectileDirect(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16 + new Vector2(8, 8), Vector2.Zero, ModContent.ProjectileType<UnstableIcicleProj>(), NPCUtils.ToActualDamage(80, 1.5f, 2f), 0);
				proj.frame = TileVariant + (tile.TileFrameX / 18 * 3);
				proj.netUpdate = true;
			}
		}

		public override bool KillSound(int i, int j, bool fail) => false;

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			if (tile.TileFrameY != 0)
				return false; //Because we're specially drawing the entire frame from one tile

			Texture2D texture = TextureAssets.Tile[Type].Value;
			Rectangle source = texture.Frame(NumStyles, 1, (tile.TileFrameX / 18 * 3) + TileVariant, 0, -2);

			Vector2 offset = Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;
			Vector2 drawPos = ((new Vector2(i, j) + offset) * 16) - Main.screenPosition - new Vector2(0, 2);

			spriteBatch.Draw(texture, drawPos, source, Lighting.GetColor(i, j), 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
			return false;
		}
	}

	public class UnstableIcicle1 : UnstableIcicle
	{
		public override int TileVariant => 1;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileCut[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.Origin = Point16.Zero;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
			TileObjectData.newTile.AnchorValidTiles = new int[] { TileID.IceBlock, TileID.SnowBlock };
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(100, 100, 60));
			DustType = -1;
		}
	}

	public class UnstableIcicle2 : UnstableIcicle
	{
		public override int TileVariant => 2;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileCut[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.Origin = Point16.Zero;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
			TileObjectData.newTile.AnchorValidTiles = new int[] { TileID.IceBlock, TileID.SnowBlock };
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(100, 100, 60));
			DustType = -1;
		}
	}
}