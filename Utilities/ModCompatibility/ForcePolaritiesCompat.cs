using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Utilities.ModCompatibility;

internal class ForcePolaritiesCompat : ModSystem
{
	public override void PostSetupRecipes()
	{
		if (ModLoader.TryGetMod("Polarities", out Mod _))
			On.Terraria.Main.DrawUnderworldBackgroudLayer += Main_DrawUnderworldBackgroudLayer;
	}

	private void Main_DrawUnderworldBackgroudLayer(On.Terraria.Main.orig_DrawUnderworldBackgroudLayer orig, bool flat, Vector2 screenOffset, float pushUp, int layerTextureIndex)
	{
		if (Main.LocalPlayer.ZoneUnderworldHeight)
			orig(flat, screenOffset, pushUp, layerTextureIndex);
	}
}
