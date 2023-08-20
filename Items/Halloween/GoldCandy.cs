using Microsoft.Xna.Framework;
using Terraria.ID;

namespace SpiritMod.Items.Halloween
{
	public class GoldCandy : CandyBase
	{
		internal override Point Size => new(40, 38);
		public override void StaticDefaults()
		{
			// DisplayName.SetDefault("Golden Candy");
			// Tooltip.SetDefault("Can't be eaten, but may sell for a lot!");
		}

		public override void Defaults()
		{
			Item.width = Size.X;
			Item.height = Size.Y;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 30;
			Item.value = 50000;
			Item.consumable = false;
		}
	}
}
