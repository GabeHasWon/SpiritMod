﻿using SpiritMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.SceneEffects
{
	internal class MeteoriteScene : ModSceneEffect
	{
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Meteor");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
		public override bool IsSceneEffectActive(Player player) => ModContent.GetInstance<SpiritMusicConfig>().MeteorMusic && player.ZoneMeteor && !Main.bloodMoon;
	}
}
