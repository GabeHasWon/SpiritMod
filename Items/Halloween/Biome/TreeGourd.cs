using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Halloween.Biome
{
	public class TreeGourd : FoodItem
	{
		internal override Point Size => new(32, 38);
		public override void StaticDefaults()
		{
			// DisplayName.SetDefault("Birdhouse Gourd");
			// Tooltip.SetDefault("Minor improvements to all stats");
		}
	}
}
