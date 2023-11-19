using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class GoldenCaviar : FoodItem
	{
		internal override Point Size => new(30, 34);

		public override void Defaults()
		{
			Item.buffType = BuffID.WellFed3;
			Item.value = Item.sellPrice(0, 2, 0, 0);
		}

        public override bool CanUseItem(Player player)
        {
            player.AddBuff(BuffID.Shine, 7200);
            return true;
        }
    }
}
