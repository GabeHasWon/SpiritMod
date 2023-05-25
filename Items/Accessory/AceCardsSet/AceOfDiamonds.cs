using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Accessory.AceCardsSet
{
	public class AceOfDiamonds : AccessoryItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ace of Diamonds");
			Tooltip.SetDefault("Critical hits have a chance to drop Diamond Aces\nDiamond aces give you 15% increased damage for 3 seconds upon collecting");
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}
	}
}
