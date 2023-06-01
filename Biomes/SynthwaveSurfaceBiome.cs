using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace SpiritMod.Biomes
{
	internal class SynthwaveSurfaceBiome : ModBiome
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Synthwave");
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("SpiritMod/SynthwaveBGStyle");
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Mushroom;
		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

		public override int Music => MusicLoader.GetMusicSlot(Mod, Main.dayTime ? "Sounds/Music/NeonTech1" : "Sounds/Music/NeonTech");

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => MapBackground;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override string MapBackground => "SpiritMod/Backgrounds/SynthwaveMapBG";

		public override bool IsBiomeActive(Player player)
		{
			bool surface = player.ZoneSkyHeight || player.ZoneOverworldHeight;
			return ModContent.GetInstance<BiomeTileCounts>().inSynthwave && surface;
		}

		public override void SpecialVisuals(Player player, bool isActive) => player.ManageSpecialBiomeVisuals("SpiritMod:SynthwaveSky", isActive);

		//public override void OnEnter(Player player) => player.GetSpiritPlayer().ZoneSynthwave = true;
		//public override void OnLeave(Player player) => player.GetSpiritPlayer().ZoneSynthwave = false;
	}
}
