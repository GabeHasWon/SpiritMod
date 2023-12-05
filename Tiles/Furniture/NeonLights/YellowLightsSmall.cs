using Microsoft.Xna.Framework;

namespace SpiritMod.Tiles.Furniture.NeonLights
{
	public class YellowLightsSmall : BlueLightsSmall
	{
		public override void StaticDefaults() => AddMapEntry(new Color(255, 243, 74), CreateMapEntryName());

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
			=> (r, g, b) = (.255f * 1.5f, .243f * 1.5f, .074f * 1.5f);
    }
}