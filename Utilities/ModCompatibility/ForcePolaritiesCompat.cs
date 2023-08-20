using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Utilities.ModCompatibility;

internal class ForcePolaritiesCompat : ModSystem
{
	public override void PostSetupRecipes()
	{
		if (ModLoader.TryGetMod("Polarities", out Mod _))
			Terraria.On_Main.DrawUnderworldBackgroudLayer += Main_DrawUnderworldBackgroudLayer;
	}

	private void Main_DrawUnderworldBackgroudLayer(Terraria.On_Main.orig_DrawUnderworldBackgroudLayer orig, bool flat, Vector2 screenOffset, float pushUp, int layerTextureIndex)
	{
		if (Main.LocalPlayer.ZoneUnderworldHeight)
			orig(flat, screenOffset, pushUp, layerTextureIndex);
	}
}
