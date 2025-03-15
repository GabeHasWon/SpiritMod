using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.AccessoriesMisc.EyeOfTheSorcerer;

public class EyeOfTheSorcererItem : ModItem
{
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

		player.GetCritChance(DamageClass.Generic) += increase;
	}
}