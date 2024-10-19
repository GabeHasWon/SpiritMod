using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Halloween
{
	public class GoldCandy : CandyBase
	{
		internal override Point Size => new(40, 38);

		public override void Defaults()
		{
			Item.width = Size.X;
			Item.height = Size.Y;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 50000;
			Item.consumable = true;
		}
	}
}
