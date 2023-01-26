using Microsoft.Xna.Framework;
using Terraria.Graphics;

namespace SpiritMod.Effects
{
	internal static class DrawTools
	{
		public delegate void ModifyColorDelegate(ref Color color);

		public static void Multiply(ref this VertexColors colors, float multiplier)
		{
			colors.TopLeftColor *= multiplier;
			colors.TopRightColor *= multiplier;
			colors.BottomLeftColor *= multiplier;
			colors.BottomRightColor *= multiplier;
		}

		public static void PerColor(ref this VertexColors colors, ModifyColorDelegate action)
		{
			action.Invoke(ref colors.TopLeftColor);
			action.Invoke(ref colors.TopRightColor);
			action.Invoke(ref colors.BottomLeftColor);
			action.Invoke(ref colors.BottomRightColor);
		}

		public static float Luminosity(this Color color) => (0.3f * (color.R / 255f)) + (0.59f * (color.G / 255f)) + (0.11f * (color.B / 255f));
	}
}
