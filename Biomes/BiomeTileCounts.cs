using SpiritMod.Items.BossLoot.StarplateDrops;
using SpiritMod.Tiles.Block;
using SpiritMod.Tiles.Furniture;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Biomes
{
	internal class BiomeTileCounts : ModSystem
	{
		public int briarCount;
		public int spiritCount;
		public int asteroidCount;
		public int marbleCount;
		public int graniteCount;
		public bool inSynthwave;

		public static bool InSpirit => ModContent.GetInstance<BiomeTileCounts>().spiritCount > 80;
		public static bool InBriar => ModContent.GetInstance<BiomeTileCounts>().briarCount > 80;
		public static bool InAsteroids => ModContent.GetInstance<BiomeTileCounts>().asteroidCount > 40;

		public static bool InMarble => ModContent.GetInstance<BiomeTileCounts>().marbleCount > 30;
		public static bool InGranite => ModContent.GetInstance<BiomeTileCounts>().graniteCount > 30;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			//Modded biomes
			briarCount = tileCounts[ModContent.TileType<BriarGrass>()];
			spiritCount = tileCounts[ModContent.TileType<Spiritsand>()] + tileCounts[ModContent.TileType<SpiritStone>()] + tileCounts[ModContent.TileType<SpiritDirt>()] + tileCounts[ModContent.TileType<SpiritGrass>()] + tileCounts[ModContent.TileType<SpiritIce>()];
			asteroidCount = tileCounts[ModContent.TileType<Asteroid>()] + tileCounts[ModContent.TileType<BigAsteroid>()] + tileCounts[ModContent.TileType<SpaceJunkTile>()] + tileCounts[ModContent.TileType<Glowstone>()];
			inSynthwave = tileCounts[ModContent.TileType<SynthwaveHeadActive>()] > 0;

			//Vanilla biomes
			marbleCount = tileCounts[TileID.Marble];
			graniteCount = tileCounts[TileID.Granite];
		}
	}
}