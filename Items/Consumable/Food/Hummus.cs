using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class Hummus : FoodItem
	{
		internal override Point Size => new(38, 28);
	}
}
