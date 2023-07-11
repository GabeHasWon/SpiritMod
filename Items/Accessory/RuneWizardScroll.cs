using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory
{
	[AutoloadEquip(EquipType.Waist)]
	public class RuneWizardScroll : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rune Wizard's Scroll");
			Tooltip.SetDefault("'We all knew skeletons could write.'\nMagic attacks may inflict random, powerful debuffs on foes\nMagic attacks may shoot out a random projectile");
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.value = Item.sellPrice(0, 1, 20, 0);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetSpiritPlayer().runeWizardScroll = true;
	}
}
