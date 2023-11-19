using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class Cake : FoodItem
	{
		internal override Point Size => new(30, 38);
	}
}
