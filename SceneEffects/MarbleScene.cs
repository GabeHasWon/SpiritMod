using SpiritMod.Biomes;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.SceneEffects
{
	internal class MarbleScene : ModSceneEffect
	{
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/MarbleBiome");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;
		public override bool IsSceneEffectActive(Player player) => BiomeTileCounts.InMarble;
	}
}
