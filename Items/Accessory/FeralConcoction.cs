using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory
{
	[AutoloadEquip(EquipType.Waist)]
	public class FeralConcoction : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brew of Immunity");
			Tooltip.SetDefault("Immunity to Feral Bite");
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.value = Item.buyPrice(0, 4, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.buffImmune[BuffID.Rabies] = true;
	}
}
