using Terraria.ModLoader;

namespace SpiritMod.Utilities.ModCompatibility.FargosCompat;

internal class FargosCompatibility : ModSystem
{
	public override void PostSetupContent()
	{
		if (ModLoader.TryGetMod("Fargowiltas", out var mutantMod))
			mutantMod.Call("AddAbominationnEvent", () => MyWorld.jellySky = false, () => MyWorld.jellySky);
	}
}
