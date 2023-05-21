using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Accessory.AceCardsSet
{
	public class AceOfSpades : AccessoryItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ace of Spades");
			Tooltip.SetDefault("Critical hits deal extra damage.");
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
