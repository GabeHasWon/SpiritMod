using SpiritMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.SceneEffects;

internal class FrostLegionScene : ModSceneEffect
{
	public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/FrostLegion");
	public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
	public override bool IsSceneEffectActive(Player player) => Main.invasionType == InvasionID.SnowLegion && ModContent.GetInstance<SpiritMusicConfig>().FrostLegionMusic && 
		player.ZoneOverworldHeight && Main.invasionProgressNearInvasion;
}
