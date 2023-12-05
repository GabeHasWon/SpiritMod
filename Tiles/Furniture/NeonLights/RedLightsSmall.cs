using Microsoft.Xna.Framework;

namespace SpiritMod.Tiles.Furniture.NeonLights
{
	public class RedLightsSmall : BlueLightsSmall
	{
		public override void StaticDefaults() => AddMapEntry(new Color(222, 31, 56), CreateMapEntryName());

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
			=> (r, g, b) = (.222f * 1.5f, .031f * 1.5f, .054f * 1.5f);
    }
}