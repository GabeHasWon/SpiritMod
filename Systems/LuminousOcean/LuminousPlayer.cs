using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Systems.LuminousOcean;

public class LuminousPlayer : ModPlayer
{
	/// <summary> Whether this player is present in a luminous ocean. </summary>
	public bool LuminousOceanActive => Player.ZoneBeach && LuminousWorld.LuminousOceanActive;

	public override void PostUpdateMiscEffects()
	{
		if (Main.dedServ)
			return;

		if (LuminousWorld.LuminousType == LuminousWorld.GREEN)
			Player.ManageSpecialBiomeVisuals("SpiritMod:GreenAlgaeSky", true);
		else if (LuminousWorld.LuminousType == LuminousWorld.BLUE)
			Player.ManageSpecialBiomeVisuals("SpiritMod:BlueAlgaeSky", true);
		else if (LuminousWorld.LuminousType == LuminousWorld.PURPLE)
			Player.ManageSpecialBiomeVisuals("SpiritMod:PurpleAlgaeSky", true);
	}
}