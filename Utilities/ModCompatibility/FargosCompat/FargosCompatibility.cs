using System;
using Terraria.ModLoader;

namespace SpiritMod.Utilities.ModCompatibility.FargosCompat;

internal class FargosCompatibility : ModSystem
{
	public override void PostSetupContent()
	{
		if (ModLoader.TryGetMod("Fargowiltas", out Mod fargos))
		{
			// AddSummon, order or value in terms of vanilla bosses, your mod internal name, summon   
			//item internal name, inline method for retrieving downed value, price to sell for in copper
			fargos.Call("AddSummon", 1.4f, "SpiritMod", "ScarabIdol", () => MyWorld.DownedScarabeus, 100 * 200);
			fargos.Call("AddSummon", 4.2f, "SpiritMod", "JewelCrown", () => MyWorld.DownedAncientAvian, 100 * 200);
			fargos.Call("AddSummon", 5.9f, "SpiritMod", "StarWormSummon", () => MyWorld.DownedStarplate, 100 * 400);
			fargos.Call("AddSummon", 6.5f, "SpiritMod", "CursedCloth", () => MyWorld.DownedInfernon, 100 * 500);
			fargos.Call("AddSummon", 7.3f, "SpiritMod", "DuskCrown", () => MyWorld.DownedDusking, 100 * 500);
			fargos.Call("AddSummon", 12.4f, "SpiritMod", "StoneSkin", () => MyWorld.DownedAtlas, 100 * 800);

			// AddAbominationnEvent, takes the disable Action and then the check Func<bool> in that order
			fargos.Call("AddAbominationnEvent", () =>
			{
				// Needs to be spaced like this in order to be an Action not a Func<bool>.
				MyWorld.jellySky = false;
			}, () => MyWorld.jellySky);
		}
	}
}
