using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class PopRocks : FoodItem
	{
		internal override Point Size => new(36, 42);

		public override bool CanUseItem(Player player)
		{
			player.AddBuff(BuffID.Shine, 7200);
			return base.CanUseItem(player);
		}
	}
}
