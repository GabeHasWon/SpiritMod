using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class Meatballs : FoodItem
	{
		internal override Point Size => new(34, 28);

		public override bool CanUseItem(Player player)
		{
			player.AddBuff(BuffID.Regeneration, 1800);
			return true;
		}
	}
}
