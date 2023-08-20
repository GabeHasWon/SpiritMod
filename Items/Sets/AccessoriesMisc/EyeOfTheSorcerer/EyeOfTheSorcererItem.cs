using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.AccessoriesMisc.EyeOfTheSorcerer;

public class EyeOfTheSorcererItem : ModItem
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Eye of the Sorcerer");
		// Tooltip.SetDefault("Gain crit chance based on the amount of mana you have\nAt full mana gain an additional 10% crit");
	}

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 30;
		Item.value = Item.sellPrice(silver: 35);
		Item.rare = ItemRarityID.Blue;
		Item.defense = 1;
		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		int increase = (int)(15 * (player.statMana / (float)player.statManaMax2));

		if (player.statMana == player.statManaMax2)
			increase += 10;

		player.GetCritChance(DamageClass.Magic) += increase;
	}
}