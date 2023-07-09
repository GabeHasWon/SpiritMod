using Terraria.ModLoader;

namespace SpiritMod.Utilities.ModCompatibility.MusicDisplayCompat;

internal class MusicDisplayCalls : ModSystem
{
	public override void PostSetupContent()
	{
		if (!ModLoader.TryGetMod("MusicDisplay", out Mod display))
			return;

		void AddMusic(string path, string name, string author = "salvati") => display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, path), name, "by " + author, "Spirit Mod");

		AddMusic("Sounds/Music/GraniteBiome", "Resounding Stones (Granite Theme)");
		AddMusic("Sounds/Music/AuroraSnow", "Aurora Borealis");
		AddMusic("Sounds/Music/DepthInvasion", "Drowning Tides (Tide Theme)");
		AddMusic("Sounds/Music/SnowNighttime", "Icy Stars (Snow Night Theme)");
		AddMusic("Sounds/Music/DesertNighttime", "Kempt Dunes (Desert Night Theme)");
		AddMusic("Sounds/Music/OceanNighttime", "Beyond the Horizon (Luminous Ocean Theme)");
		AddMusic("Sounds/Music/CrimsonNighttime", "Razor's Edge (Crimson Night Theme)", "salvati, sbubby");
		AddMusic("Sounds/Music/ReachUnderground", "Lurking Vines (Underground Briar Theme)", "sbubby");
		AddMusic("Sounds/Music/Meteor", "Space Rocks! (Meteorite Theme)");
		AddMusic("Sounds/Music/Infernon", "Fiery Care (Infernon Theme)");
		AddMusic("Sounds/Music/CorruptNight", "Nightmare Fuel (Corruption Night Theme)");
		AddMusic("Sounds/Music/NeonTech1", "Spaced Out (Hyperspace Day Theme)");
		AddMusic("Sounds/Music/NeonTech", "Techno Vibes (Hyperspace Night Theme)");
		AddMusic("Sounds/Music/BlueMoon", "Nighttime Groove (Mystic Moon Theme)");
		AddMusic("Sounds/Music/JellySky", "Sticky Situation (Jelly Deluge Theme)");
		AddMusic("Sounds/Music/FrostLegion", "Tougher Chill (Frost Legion Theme)");
		AddMusic("Sounds/Music/SpiderCave", "Xyresic Horrors (Spider Cave Theme)");
		AddMusic("Sounds/Music/MoonJelly", "Mystical Mischief (Moon Jelly Wizard Theme)");
		AddMusic("Sounds/Music/HallowNight", "Wonderous Landscape (Hallow Night Theme)");
		AddMusic("Sounds/Music/Atlas", "Tremors Underneath (Atlas Theme)");
		AddMusic("Sounds/Music/SpiritLayer3", "Hellish Atmosphere (Spirit Depths Theme)");
		AddMusic("Sounds/Music/ReachBoss", "Monarch Husk (Vinewrath Bane Theme)");
		AddMusic("Sounds/Music/MarbleBiome", "Regal Tunnel (Marble Theme)");
		AddMusic("Sounds/Music/Blizzard", "Shivering Storms (Blizzard Theme)");
		AddMusic("Sounds/Music/SpiritLayer1", "Fearful Atmosphere (Underground Spirit Theme)");
		AddMusic("Sounds/Music/Starplate", "Lost Voyager (Starplate Voyager Theme)");
		AddMusic("Sounds/Music/SpiritOverworld", "Enervating Atmosphere (Spirit Theme)");
		AddMusic("Sounds/Music/Reach", "Floral Fiends (Briar Theme)");
		AddMusic("Sounds/Music/AncientAvian", "Jettisoned Giant (Ancient Avian Theme)");
		AddMusic("Sounds/Music/UnderwaterMusic", "Deep Ocean");
		AddMusic("Sounds/Music/VictoryDay", "Overworld Alt Alt (Victory Day Theme)");
		AddMusic("Sounds/Music/AshStorm", "Ash Storm");
		AddMusic("Sounds/Music/Asteroids", "Above Islands (Asteroid Theme)");
		AddMusic("Sounds/Music/DuskingTheme", "Dusking", "LordCakeSpy");
		AddMusic("Sounds/Music/Scarabeus", "Crawling Complications (Scarabeus Theme)");
		AddMusic("Sounds/Music/TranquilWinds", "Tranquil Winds");
	}
}
