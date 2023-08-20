using Terraria.ModLoader;

namespace SpiritMod.Biomes.Events
{
	internal class JellyDelugeBiome : ModBiome //This is used solely for the bestiary
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Jelly Deluge");
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => "SpiritMod/Backgrounds/JellyDelugeBestiaryBG";
	}
}
