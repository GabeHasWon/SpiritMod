using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Utilities.ModCompatibility.MusicDisplayCompat;

internal class MusicDisplayCalls : ModSystem
{
	public override void PostSetupContent()
	{
		if (!ModLoader.TryGetMod("MusicDisplay", out Mod display))
			return;

		LocalizedText modName = Language.GetText("Mods.SpiritMod.MusicDisplay.ModName");

		void AddMusic(string name)
		{
			LocalizedText author = Language.GetText("Mods.SpiritMod.MusicDisplay." + name + ".Author");
			LocalizedText displayName = Language.GetText("Mods.SpiritMod.MusicDisplay." + name + ".DisplayName");
			display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, "Sounds/Music/" + name), displayName, author, modName);
		}

		AddMusic("GraniteBiome");
		AddMusic("AuroraSnow");
		AddMusic("DepthInvasion");
		AddMusic("SnowNighttime");
		AddMusic("DesertNighttime");
		AddMusic("OceanNighttime");
		AddMusic("CrimsonNighttime");
		AddMusic("ReachUnderground");
		AddMusic("Meteor");
		AddMusic("Infernon");
		AddMusic("CorruptNight");
		AddMusic("NeonTech1");
		AddMusic("NeonTech");
		AddMusic("BlueMoon");
		AddMusic("JellySky");
		AddMusic("FrostLegion");
		AddMusic("SpiderCave");
		AddMusic("MoonJelly");
		AddMusic("HallowNight");
		AddMusic("Atlas");
		AddMusic("SpiritLayer3");
		AddMusic("ReachBoss");
		AddMusic("MarbleBiome");
		AddMusic("Blizzard");
		AddMusic("SpiritLayer1");
		AddMusic("Starplate");
		AddMusic("SpiritOverworld");
		AddMusic("Reach");
		AddMusic("AncientAvian");
		AddMusic("UnderwaterMusic");
		AddMusic("VictoryDay");
		AddMusic("AshStorm");
		AddMusic("Asteroids");
		AddMusic("DuskingTheme");
		AddMusic("Scarabeus");
		AddMusic("TranquilWinds");
		AddMusic("ReachNighttime");
	}
}
