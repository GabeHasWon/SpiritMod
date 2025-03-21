using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Systems.LuminousOcean;

public class LuminousPlayer : ModPlayer
{
	/// <summary> Whether this player is present in a luminous ocean at night. </summary>
	public bool LuminousOceanActive => Player.ZoneBeach && !Main.dayTime && LuminousWorld.LuminousOceanActive;

	public override void PostUpdateMiscEffects()
	{
		if (Main.dedServ || Player.whoAmI != Main.myPlayer)
			return;

		Player.ManageSpecialBiomeVisuals("SpiritMod:GreenAlgaeSky", Matching(LuminousWorld.GREEN));
		Player.ManageSpecialBiomeVisuals("SpiritMod:BlueAlgaeSky", Matching(LuminousWorld.BLUE));
		Player.ManageSpecialBiomeVisuals("SpiritMod:PurpleAlgaeSky", Matching(LuminousWorld.PURPLE));

		bool Matching(int type) => LuminousOceanActive && LuminousWorld.LuminousType == type;
	}
}