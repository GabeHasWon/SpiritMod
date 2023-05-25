using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Accessory.AceCardsSet
{
	public class AceOfHearts : AccessoryItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ace of Hearts");
			Tooltip.SetDefault("Critical hits have a chance to drop hearts");
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
