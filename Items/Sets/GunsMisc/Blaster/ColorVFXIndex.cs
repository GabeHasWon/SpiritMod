using Microsoft.Xna.Framework;
using Terraria.ID;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	internal static class ColorEffectsIndex
	{
		internal static Color GetColor(int index)
		{
			return index switch
			{
				1 => new Color(22, 245, 140),
				2 => new Color(158, 158, 225),
				3 => new Color(250, 60, 225),
				_ => new Color(255, 230, 65)
			};
		}

		internal static int[] GetDusts(int index)
		{
			return index switch
			{
				1 => new int[] { DustID.FartInAJar, DustID.GreenTorch },
				2 => new int[] { DustID.FrostHydra, DustID.IceTorch },
				3 => new int[] { DustID.Pixie, DustID.PinkTorch },
				_ => new int[] { DustID.SolarFlare, DustID.Torch }
			};
		}

		internal static int? GetDebuffs(int index)
		{
			return index switch
			{
				1 => BuffID.Poisoned,
				2 => BuffID.Frostburn,
				3 => null,
				_ => BuffID.OnFire
			};
		}
	}
}