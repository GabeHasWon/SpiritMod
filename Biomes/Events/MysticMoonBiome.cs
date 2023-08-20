using Terraria.ModLoader;

namespace SpiritMod.Biomes.Events
{
	internal class MysticMoonBiome : ModBiome //This is used solely for the bestiary
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Mystic Moon");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => "SpiritMod/Backgrounds/MysticMoonBestiaryBG";
	}
}
