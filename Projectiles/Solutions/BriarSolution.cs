using System;
using System.Linq;
using Microsoft.Xna.Framework;
using SpiritMod.Tiles.Ambient.Briar;
using SpiritMod.Tiles.Block;
using SpiritMod.Tiles.Walls.Natural;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Solutions
{
	public class BriarSolution : SpiritSolution
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Briar Spray");
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
		}

		public override bool? CanCutTiles() => false;

		protected override void DoDusts(float scale)
		{
			Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.PoisonStaff, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100, new Color(), scale);
			dust.noGravity = true;
		}

		protected override void Convert(int i, int j, int size = 4)
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

					if (WallID.Sets.Conversion.Grass[tile.WallType] && tile.WallType != (ushort)ModContent.WallType<ReachWallNatural>())
					{
						tile.WallType = (ushort)ModContent.WallType<ReachWallNatural>();
						WorldGen.SquareWallFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}

					if (TileID.Sets.Conversion.Grass[tile.TileType] && tile.TileType != (ushort)ModContent.TileType<BriarGrass>())
					{
						tile.TileType = (ushort)ModContent.TileType<BriarGrass>();

						//Convert top plants
						if (plantIndex.Contains(aboveTile.TileType) && tile.TileType != (ushort)ModContent.TileType<BriarFoliage>())
						{
							aboveTile.TileType = (ushort)ModContent.TileType<BriarFoliage>();
							if (aboveTile.TileFrameX > 126)
								aboveTile.TileFrameX = 126;

							WorldGen.SquareTileFrame(k, l - 1);
							NetMessage.SendTileSquare(-1, k, l - 1, 1);
						}
						WorldGen.SquareTileFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}
					else if (tile.TileType == ModContent.TileType<SpiritDirt>())
					{
						tile.TileType = (ushort)TileID.Dirt;
						WorldGen.SquareTileFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}
				}
			}
		}

	}
}
