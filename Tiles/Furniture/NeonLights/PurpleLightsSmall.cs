using Microsoft.Xna.Framework;

namespace SpiritMod.Tiles.Furniture.NeonLights
{
	public class PurpleLightsSmall : BlueLightsSmall
	{
		public override void StaticDefaults() => AddMapEntry(new Color(139, 88, 255), CreateMapEntryName());

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
			=> (r, g, b) = (.139f * 1.75f, .077f * 1.75f, .255f * 1.75f);
    }
}