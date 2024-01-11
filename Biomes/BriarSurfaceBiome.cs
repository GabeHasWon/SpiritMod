using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace SpiritMod.Biomes
{
	internal class BriarSurfaceBiome : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("SpiritMod/ReachWaterStyle");
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("SpiritMod/ReachSurfaceBgStyle");
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
		
		public override int Music => Main.LocalPlayer.townNPCs >= 2 ? -1 : (Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Reach") : MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ReachNighttime"));

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => MapBackground;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override string MapBackground => "SpiritMod/Backgrounds/BriarMapBG";

		public override bool IsBiomeActive(Player player)
		{
			bool surface = player.ZoneSkyHeight || player.ZoneOverworldHeight;
			return BiomeTileCounts.InBriar && surface;
		}
	}
}
