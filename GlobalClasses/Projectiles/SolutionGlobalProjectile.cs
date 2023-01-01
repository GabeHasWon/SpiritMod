using SpiritMod.Tiles.Ambient.Briar;
using SpiritMod.Tiles.Ambient.Spirit;
using SpiritMod.Tiles.Block;
using SpiritMod.Tiles.Walls.Natural;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Projectiles
{
	public class SolutionGlobalProjectile : GlobalProjectile
	{
		private readonly int[] plantIndex = new int[] { ModContent.TileType<SpiritFoliage>(), ModContent.TileType<BriarFoliage>() };
		private readonly int[] grassIndex = new int[] { TileID.Grass, TileID.CorruptGrass, TileID.CrimsonGrass, TileID.HallowedGrass };

		private readonly int[] solutions = new int[] { ProjectileID.PureSpray, ProjectileID.CorruptSpray, ProjectileID.CrimsonSpray, ProjectileID.HallowSpray };

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => solutions.Contains(entity.type);

		public override void AI(Projectile projectile)
		{
			base.AI(projectile);

			int size = 4;
			int i = (int)(projectile.position.X + projectile.width / 2f) / 16;
			int j = (int)(projectile.position.Y + projectile.height / 2f) / 16;

			for (int k = i - size; k <= i + size; k++)
			{
				for (int l = j - size; l <= j + size; l++)
				{
					if (!WorldGen.InWorld(k, l, 1) ||
					!(Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt(size * size + size * size)))
						continue;

					Tile tile = Framing.GetTileSafely(k, l);
					Tile aboveTile = Framing.GetTileSafely(k, l - 1);

					//This check shouldn't be necessary
					if (/*WallID.Sets.Conversion.Grass[tile.WallType]*/ tile.WallType == ModContent.WallType<SpiritWallNatural>() || tile.WallType == ModContent.WallType<ReachWallNatural>())
					{
						tile.WallType = (ushort)WallID.Grass;
						WorldGen.SquareTileFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}

					if (tile.TileType == ModContent.TileType<SpiritDirt>())
					{
						tile.TileType = (ushort)TileID.Dirt;
						WorldGen.SquareTileFrame(k, l);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}

					//Convert small plants
					if (grassIndex.Contains(tile.TileType) && plantIndex.Contains(aboveTile.TileType))
					{
						int type = Array.IndexOf(solutions, projectile.type) switch
						{
							0 => TileID.Plants,
							1 => TileID.CorruptPlants,
							2 => TileID.CrimsonPlants,
							3 => TileID.HallowedPlants,
							_ => -1
						};

						if (type == -1)
							continue;

						aboveTile.TileType = (ushort)type;
						WorldGen.SquareTileFrame(k, l - 1);
						NetMessage.SendTileSquare(-1, k, l - 1, 1);
					}
				}
			}

		}
	}
}