using Microsoft.Xna.Framework;
using SpiritMod.Skies.Overlays;
using SpiritMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Systems.Aurora;

internal class AuroraPlayer : ModPlayer
{
	/// <summary> Whether this player is present in a natural aurora.</summary>
	public bool AuroraActive => GetAuroraType() != -1;
	/// <summary> Whether an aurora is <b>visually</b> present. This considers whether the player is under the effects of a monolith in addition to natural auroras. </summary>
	public bool InLocalAurora => auroraVisuals.Any(x => x.Value > 0);

	private const byte Duration = 30;
	private int _priority = -1;

	private readonly Dictionary<int, byte> auroraVisuals = new()
	{
		{ AuroraOverlay.UNUSED_BASIC, 0 }, { AuroraOverlay.PRIMARY, 0 }, { AuroraOverlay.PRIMARY_ALT1, 0 },
		{ AuroraOverlay.PRIMARY_ALT2, 0 }, { AuroraOverlay.PRIMARY_ALT3, 0 }, { AuroraOverlay.BLOODMOON, 0 },
		{ AuroraOverlay.PUMPKINMOON, 0 }, { AuroraOverlay.FROSTMOON, 0 }, { AuroraOverlay.BLUEMOON, 0 },
		{ AuroraOverlay.SPIRIT, 0 }
	};

	/// <summary> Activates a local aurora of the given type. </summary>
	public void SetLocalAurora(int type)
	{
		if (_priority == -1)
		{
			auroraVisuals[type] = (byte)Math.Min(auroraVisuals[type] + 1, Duration);
			_priority = type;
		}
	}

	public float GetAuroraIntensity(int type) => MathHelper.Clamp(auroraVisuals[type] / (float)Duration * 1.25f, 0, 1);

	public void Reset()
	{
		for (int i = 0; i < AuroraOverlay.COUNT; i++) //Reset aurora monolith values
		{
			if (i == AuroraOverlay.COMPLETELY_UNIMPLEMENTED || i == _priority)
				continue;

			auroraVisuals[i] = (byte)Math.Max(auroraVisuals[i] - 1, 0);
		}

		_priority = -1;
	}

	public override void PostUpdateMiscEffects()
	{
		if (Main.dedServ)
			return;

		int type = GetAuroraType();

		if (type != -1)
			SetLocalAurora(type);

		Player.ManageSpecialBiomeVisuals("SpiritMod:AuroraSky", InLocalAurora);
	}

	private int GetAuroraType()
	{
		if (Main.dayTime || !AuroraWorld.AuroraActive || (Main.raining && !Player.ZoneSnow) || Player.ZoneCorrupt || Player.ZoneCrimson)
			return -1;

		if (Main.bloodMoon)
			return AuroraOverlay.BLOODMOON;

		if (Main.pumpkinMoon)
			return AuroraOverlay.PUMPKINMOON;

		if (Main.snowMoon)
			return AuroraOverlay.FROSTMOON;

		if (MyWorld.blueMoon)
			return AuroraOverlay.BLUEMOON;

		if (Player.ZoneSpirit())
			return AuroraOverlay.SPIRIT;

		if (Player.ZoneSnow || Player.ZoneSkyHeight)
			return AuroraWorld.AuroraType;

		return -1;
	}
}