using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Buffs;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class Popsicle : FoodItem
	{
		internal override Point Size => new(20, 42);
		public override void StaticDefaults() => Tooltip.SetDefault("Minor improvements to all stats\nGrants immunity to 'On Fire'\n'Eat it before it melts!'");

		public override bool CanUseItem(Player player)
		{
			player.AddBuff(ModContent.BuffType<IceBerryBuff>(), 7200);
			return true;
		}

	}
}
