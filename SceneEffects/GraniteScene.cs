using SpiritMod.Biomes;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.SceneEffects
{
	internal class GraniteScene : ModSceneEffect
	{
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/GraniteBiome");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;
		public override bool IsSceneEffectActive(Player player) => BiomeTileCounts.InGranite;
	}
}
