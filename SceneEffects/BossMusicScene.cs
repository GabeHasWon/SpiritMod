using SpiritMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.SceneEffects
{
	internal class BossMusicScene : ModSceneEffect
	{
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SkeletronPrime");
		public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
		public override bool IsSceneEffectActive(Player player) => ModContent.GetInstance<SpiritMusicConfig>().SkeletronPrimeMusic && NPC.AnyNPCs(NPCID.SkeletronPrime);
	}
}
