using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.SceneEffects
{
	internal class VictoryDayScene : ModSceneEffect
	{
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VictoryDay");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;
		public override bool IsSceneEffectActive(Player player) => player.ZoneOverworldHeight && MyWorld.VictoryDay && ModContent.GetInstance<Utilities.SpiritMusicConfig>().VictoryDayMusic;
	}
}
