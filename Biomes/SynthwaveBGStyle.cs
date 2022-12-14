using Terraria.ModLoader;

namespace SpiritMod.Biomes
{
	public class SynthwaveBGStyle : ModSurfaceBackgroundStyle
	{
		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			scale *= .45f;
			b -= 350;
			return ModContent.GetModBackgroundSlot("SpiritMod/Backgrounds/SynthwaveBackground");
		}

		public override void ModifyFarFades(float[] fades, float transitionSpeed)
		{
			for (int i = 0; i < fades.Length; i++)
			{
				if (i == Slot)
				{
					fades[i] += transitionSpeed;

					if (fades[i] > 1f)
						fades[i] = 1f;
				}
				else
				{
					fades[i] -= transitionSpeed;

					if (fades[i] < 0f)
						fades[i] = 0f;
				}
			}
		}
	}
}