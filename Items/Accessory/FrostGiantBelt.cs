using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory
{
	[AutoloadEquip(EquipType.Waist)]
	public class FrostGiantBelt : AccessoryItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Giant Belt");
			// Tooltip.SetDefault("50% knockback resistance while charging a club\nCharging a club gradually increases defense");
		}

		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 40;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}
	}
}
