using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace SpiritMod.Biomes
{
	internal class SpiritSurfaceBiome : ModBiome
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Spirit");
		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("SpiritMod/SpiritWaterStyle");
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("SpiritMod/SpiritSurfaceBgStyle");
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Mushroom;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SpiritOverworld");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => MapBackground;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override string MapBackground => "SpiritMod/Backgrounds/SpiritMapBackground";

		public override bool IsBiomeActive(Player player)
		{
			bool surface = player.ZoneSkyHeight || player.ZoneOverworldHeight;
			return BiomeTileCounts.InSpirit && surface;
		}

		//public override void OnEnter(Player player) => player.GetSpiritPlayer().ZoneSpirit = true;
		//public override void OnLeave(Player player) => player.GetSpiritPlayer().ZoneSpirit = false;
	}
}
