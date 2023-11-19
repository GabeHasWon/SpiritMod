using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class TofuSatay : FoodItem
	{
		internal override Point Size => new(28, 44);
	}
}
