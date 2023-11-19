using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using SpiritMod.Buffs;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class Popsicle : FoodItem
	{
		internal override Point Size => new(20, 42);

		public override bool CanUseItem(Player player)
		{
			player.AddBuff(ModContent.BuffType<IceBerryBuff>(), 7200);
			return true;
		}
	}
}
