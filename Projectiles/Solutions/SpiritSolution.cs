using System;
using System.Linq;
using Microsoft.Xna.Framework;
using SpiritMod.Tiles.Ambient.Briar;
using SpiritMod.Tiles.Ambient.Spirit;
using SpiritMod.Tiles.Block;
using SpiritMod.Tiles.Walls.Natural;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Solutions
{
	public class SpiritSolution : ModProjectile
	{
		protected readonly int[] plantIndex = new int[] { TileID.Plants, TileID.CrimsonPlants, TileID.CorruptPlants, TileID.HallowedPlants, ModContent.TileType<SpiritFoliage>(), ModContent.TileType<BriarFoliage>() };

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Spirit Spray");
		public override void SetDefaults()
		{
			Projectile.width = 6;
			Projectile.height = 6;
			Projectile.friendly = true;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			Projectile.extraUpdates = 2;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 133;
		}

		public override bool? CanCutTiles() => false;

		public override void AI()
		{
			if (Projectile.owner == Main.myPlayer)
				Convert((int)(Projectile.position.X + Projectile.width / 2f) / 16, (int)(Projectile.position.Y + Projectile.height / 2f) / 16, 2);

			int minFXTime = 8;
			if (Projectile.ai[0] >= minFXTime) 
			{
				float dustScale = MathHelper.Clamp((Projectile.ai[0] >= minFXTime) ? (Projectile.ai[0] - minFXTime) / 5 : 0f, 0f, 1f) * 1.75f;
				DoDusts(dustScale);
			}
			Projectile.ai[0] += 1f;

			Projectile.rotation += 0.3f * Projectile.direction;
		}

		protected virtual void DoDusts(float scale)
		{
			Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100, new Color(), scale);
			dust.noGravity = true;
		}

		protected virtual void Convert(int i, int j, int size = 4)
		{
			for (int k = i - size; k <= i + size; k++)
			{
				for (int l = j - size; l <= j + size; l++)
				{
					if (!WorldGen.InWorld(k, l, 1) ||
						!(Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt(size * size + size * size)))
						continue;

					Tile tile = Framing.GetTileSafely(k, l);
					Tile aboveTile = Framing.GetTileSafely(k, l - 1);

					if (WallID.Sets.Conversion.Grass[tile.WallType] && tile.WallType != (ushort)ModContent.WallType<SpiritWallNatural>())
					{
						tile.WallType = (ushort)ModContent.WallType<SpiritWallNatural>();
						WorldGen.SquareWallFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}

					if ((TileID.Sets.Conversion.Stone[tile.TileType] || TileID.Sets.Conversion.Moss[tile.TileType]) && tile.TileType != (ushort)ModContent.TileType<SpiritStone>())
					{
						tile.TileType = (ushort)ModContent.TileType<SpiritStone>();
						WorldGen.SquareTileFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}

					if (tile.TileType == TileID.Dirt)
					{
						tile.TileType = (ushort)ModContent.TileType<SpiritDirt>();
						WorldGen.SquareTileFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}
					else if (TileID.Sets.Conversion.Grass[tile.TileType] && tile.TileType != (ushort)ModContent.TileType<SpiritGrass>())
					{
						tile.TileType = (ushort)ModContent.TileType<SpiritGrass>();

						//Convert top plants
						if (plantIndex.Contains(aboveTile.TileType) && tile.TileType != (ushort)ModContent.TileType<SpiritFoliage>())
						{
							aboveTile.TileType = (ushort)ModContent.TileType<SpiritFoliage>();
							if (aboveTile.TileFrameX > 270)
								aboveTile.TileFrameX = 270;

							WorldGen.SquareTileFrame(k, l - 1);
							NetMessage.SendTileSquare(-1, k, l - 1, 1);
						}
						WorldGen.SquareTileFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}
					else if (TileID.Sets.Conversion.Sand[tile.TileType])
					{
						tile.TileType = (ushort)ModContent.TileType<Spiritsand>();
						WorldGen.SquareTileFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}
					else if (TileID.Sets.Conversion.Ice[tile.TileType])
					{
						tile.TileType = (ushort)ModContent.TileType<SpiritIce>();
						WorldGen.SquareTileFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}

				}
			}
		}

	}
}