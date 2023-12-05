using Microsoft.Xna.Framework;

namespace SpiritMod.Tiles.Furniture.NeonLights
{
	public class GreenLightsSmall : BlueLightsSmall
	{
		public override void StaticDefaults() => AddMapEntry(new Color(77, 255, 88), CreateMapEntryName());

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
			=> (r, g, b) = (.077f * 1.75f, .255f * 1.75f, .088f * 1.75f);
    }
}