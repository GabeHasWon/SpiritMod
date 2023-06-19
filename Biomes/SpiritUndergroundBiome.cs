using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Biomes
{
	internal class SpiritUndergroundBiome : ModBiome
	{
		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("SpiritMod/SpiritUgBgStyle");
		public override void SetStaticDefaults() => DisplayName.SetDefault("Underground Spirit");

		public override int Music => GetMusicFromDepth();
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => MapBackground;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override string MapBackground => "SpiritMod/Backgrounds/SpiritMapBackground";

		public static int ThirdLayerHeight = (int)(Main.maxTilesY - (Main.maxTilesY * .25f));

		private int GetMusicFromDepth()
		{
			Player player = Main.LocalPlayer;
			int music;

			if (player.position.Y / 16 >= ThirdLayerHeight)
				music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SpiritLayer3");
			else if (player.ZoneRockLayerHeight)
				music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SpiritLayer2");
			else
				music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SpiritLayer1");

			return music;
		}

		public override bool IsBiomeActive(Player player) => (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InSpirit;

		//public override void OnEnter(Player player) => player.GetSpiritPlayer().ZoneSpirit = true;
		//public override void OnLeave(Player player) => player.GetSpiritPlayer().ZoneSpirit = false;
	}
}
