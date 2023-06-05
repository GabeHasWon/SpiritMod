using SpiritMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.SceneEffects
{
	internal class NightThemeScene : ModSceneEffect
	{
		private static SpiritMusicConfig Config => ModContent.GetInstance<SpiritMusicConfig>();
		private static Player Player => Main.LocalPlayer;

		private static bool ValidCorruption => Config.CorruptNightMusic && Player.ZoneCorrupt && Player.ZoneOverworldHeight
				&& !Main.dayTime && !Player.ZoneHallow && !Player.ZoneMeteor && !Main.bloodMoon;
		private static bool ValidOcean => Config.LuminousMusic && Player.ZoneBeach && MyWorld.luminousOcean && !Main.dayTime;
		private static bool ValidHallow => Config.HallowNightMusic && Player.ZoneHallow && Player.ZoneOverworldHeight && !Main.dayTime && !Main.raining && !Main.bloodMoon
				&& !Player.ZoneCorrupt && !Player.ZoneCrimson && !Player.ZoneJungle && !Player.ZoneBeach && !Player.ZoneMeteor;
		private static bool ValidCrimson => Config.CrimsonNightMusic && Player.ZoneCrimson && Player.ZoneOverworldHeight && !Main.dayTime
				&& !Player.ZoneHallow && !Player.ZoneMeteor && !Main.bloodMoon;
		private static bool ValidSnow => Config.SnowNightMusic && Player.ZoneSnow && Player.ZoneOverworldHeight && !Main.dayTime && !Player.ZoneCorrupt
				&& !Player.ZoneMeteor && !Player.ZoneCrimson && !Player.ZoneHallow && !MyWorld.aurora && !Main.raining && !Main.bloodMoon;
		private static bool ValidDesert => Config.DesertNightMusic && Player.ZoneDesert && Player.ZoneOverworldHeight && !Main.dayTime && !Player.ZoneCorrupt
				&& !Player.ZoneCrimson && !Player.ZoneBeach;

		public override int Music => GetMusic();

		public int GetMusic()
		{
			int music = -1;

			if (ValidOcean)
				music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/OceanNighttime");

			if (ValidHallow)
				music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/HallowNight");

			if (ValidCorruption)
				music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CorruptNight");

			if (ValidCrimson)
				music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrimsonNight");

			if (ValidSnow)
				music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SnowNighttime");

			if (ValidDesert)
				music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/DesertNighttime");
			return music;
		}

		public override SceneEffectPriority Priority => ValidCorruption || ValidCrimson ? SceneEffectPriority.BiomeHigh : SceneEffectPriority.BiomeMedium;
		public override bool IsSceneEffectActive(Player player) => ValidOcean || ValidHallow || ValidCrimson || ValidCorruption || ValidSnow || ValidDesert;
	}
}
