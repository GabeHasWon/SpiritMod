using Terraria.ModLoader;

namespace SpiritMod.Biomes.Events
{
	internal class TideBiome : ModBiome //This is used solely for the bestiary
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("The Tide");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => "SpiritMod/Backgrounds/TideBestiaryBG";
	}
}
