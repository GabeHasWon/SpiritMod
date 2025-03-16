using SpiritMod.Systems.Aurora;
using SpiritMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.SceneEffects;

/// <summary> Handles aurora scene music, specifically. </summary>
internal class AuroraScene : ModSceneEffect
{
	public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/AuroraSnow");
	public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
	public override bool IsSceneEffectActive(Player player) => ModContent.GetInstance<SpiritMusicConfig>().AuroraMusic
			&& player.ZoneAurora()
			&& player.ZoneSnow && player.ZoneOverworldHeight
			&& !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneMeteor
			&& !Main.bloodMoon && !Main.dayTime;
}
