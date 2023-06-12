using Terraria.ModLoader;

namespace SpiritMod.Utilities.ModCompatibility.MusicDisplayCompat;

internal class MusicDisplayCalls : ModSystem
{
	public override void PostSetupContent()
	{
		if (!ModLoader.TryGetMod("MusicDisplay", out Mod display))
			return;

		void AddMusic(string path, string name) => display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, path), name, "Spirit Mod");

		AddMusic("Sounds/Music/GraniteBiome", "salvati - Resounding Stones (Granite Theme)");
		AddMusic("Sounds/Music/AuroraSnow", "salvati - Aurora Borealis");
		AddMusic("Sounds/Music/DepthInvasion", "salvati - Drowning Tides (Tide Theme)");
		AddMusic("Sounds/Music/SnowNighttime", "salvati - Icy Stars (Snow Night Theme)");
		AddMusic("Sounds/Music/DesertNighttime", "salvati - Kempt Dunes (Desert Night Theme)");
		AddMusic("Sounds/Music/OceanNighttime", "salvati - Beyond the Horizon (Luminous Ocean Theme)");
		AddMusic("Sounds/Music/CrimsonNighttime", "salvati, sbubby - Razor's Edge (Crimson Night Theme)");
		AddMusic("Sounds/Music/ReachUnderground", "sbubby - Lurking Vines (Underground Briar Theme)");
		AddMusic("Sounds/Music/Meteor", "salvati - Space Rocks! (Meteorite Theme)");
		AddMusic("Sounds/Music/Infernon", "salvati - Fiery Care (Infernon Theme)");
		AddMusic("Sounds/Music/CorruptNight", "salvati - Nightmare Fuel (Corruption Night Theme)");
		AddMusic("Sounds/Music/NeonTech1", "salvati - Spaced Out (Hyperspace Day Theme)");
		AddMusic("Sounds/Music/NeonTech", "salvati - Techno Vibes (Hyperspace Night Theme)");
		AddMusic("Sounds/Music/BlueMoon", "salvati - Nighttime Groove (Mystic Moon Theme)");
		AddMusic("Sounds/Music/JellySky", "salvati - Sticky Situation (Jelly Deluge Theme)");
		AddMusic("Sounds/Music/FrostLegion", "salvati - Tougher Chill (Frost Legion Theme)");
		AddMusic("Sounds/Music/SpiderCave", "salvati - Xyresic Horrors (Spider Cave Theme)");
		AddMusic("Sounds/Music/MoonJelly", "salvati - Mystical Mischief (Moon Jelly Wizard Theme)");
		AddMusic("Sounds/Music/HallowNight", "salvati - Wonderous Landscape (Hallow Night Theme)");
		AddMusic("Sounds/Music/Atlas", "salvati - Tremors Underneath (Atlas Theme)");
		AddMusic("Sounds/Music/SpiritLayer3", "salvati - Hellish Atmosphere (Spirit Depths Theme)");
		AddMusic("Sounds/Music/ReachBoss", "salvati - Monarch Husk (Vinewrath Bane Theme)");
		AddMusic("Sounds/Music/MarbleBiome", "salvati - Regal Tunnel (Marble Theme)");
		AddMusic("Sounds/Music/Blizzard", "salvati - Shivering Storms (Blizzard Theme)");
		AddMusic("Sounds/Music/SpiritLayer1", "salvati - Fearful Atmosphere (Underground Spirit Theme)");
		AddMusic("Sounds/Music/Starplate", "salvati - Lost Voyager (Starplate Voyager Theme)");
		AddMusic("Sounds/Music/SpiritOverworld", "salvati - Enervating Atmosphere (Spirit Theme)");
		AddMusic("Sounds/Music/Reach", "salvati - Floral Fiends (Briar Theme)");
		AddMusic("Sounds/Music/AncientAvian", "salvati - Jettisoned Giant (Ancient Avian Theme)");
		AddMusic("Sounds/Music/UnderwaterMusic", "salvati - Deep Ocean");
		AddMusic("Sounds/Music/VictoryDay", "salvati - Overworld Alt Alt (Victory Day Theme)");
		AddMusic("Sounds/Music/AshStorm", "salvati - Ash Storm");
		AddMusic("Sounds/Music/Asteroids", "salvati - Above Islands (Asteroid Theme)");
		AddMusic("Sounds/Music/DuskingTheme", "LordCakeSpy - Dusking");
		AddMusic("Sounds/Music/Scarabeus", "salvati - Crawling Complications (Scarabeus Theme)");
		AddMusic("Sounds/Music/TranquilWinds", "salvati - Tranquil Winds");

	}
}
