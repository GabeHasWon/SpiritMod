using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Biomes
{
	internal class BriarUndergroundBiome : ModBiome
	{
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ReachUnderground");
		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("SpiritMod/ReachWaterStyle");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => MapBackground;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override string MapBackground => "SpiritMod/Backgrounds/BriarUndergroundMapBG";

		public override bool IsBiomeActive(Player player) => (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InBriar;
	}
}
