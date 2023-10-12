using SpiritMod.Buffs.Armor;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class RogueHood : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 18;
			Item.value = Terraria.Item.buyPrice(0, 0, 50, 0);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 1;
		}

		public override void UpdateEquip(Player player) => player.moveSpeed += .04f;

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type == ModContent.ItemType<RoguePlate>() && legs.type == ModContent.ItemType<RoguePants>();

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Rogue");
			player.GetSpiritPlayer().rogueSet = true;

			if (player.HasBuff(ModContent.BuffType<RogueCooldown>()) && player.HasBuff(BuffID.Invisibility))
				player.GetDamage(DamageClass.Generic) += 1f;
		}
	}
}