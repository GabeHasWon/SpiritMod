using Terraria;
using Terraria.GameContent.Events;
using Terraria.ModLoader;

namespace SpiritMod.SceneEffects
{
	internal class VictoryNightScene : ModSceneEffect
	{
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VictoryNight");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;
		public override bool IsSceneEffectActive(Player player) => player.ZoneOverworldHeight && LanternNight.LanternsUp && ModContent.GetInstance<Utilities.SpiritMusicConfig>().VictoryNightMusic;
	}
}
