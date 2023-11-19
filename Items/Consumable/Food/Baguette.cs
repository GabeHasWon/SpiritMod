using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class Baguette : FoodItem
	{
		internal override Point Size => new(32, 32);
	}
}
