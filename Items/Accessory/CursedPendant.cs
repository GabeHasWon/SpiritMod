using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory
{
	[AutoloadEquip(EquipType.Neck)]
	public class CursedPendant : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Pendant");
			Tooltip.SetDefault("6% increased melee damage\nWeapons have a 15% chance to inflict Cursed Inferno");
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetSpiritPlayer().cursedPendant = true;
			player.GetDamage(DamageClass.Melee) *= 1.06f;
		}
	}
}
