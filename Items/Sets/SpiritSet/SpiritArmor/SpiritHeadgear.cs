using SpiritMod.Items.Sets.SpiritSet;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SpiritSet.SpiritArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class SpiritHeadgear : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.value = 40000;
			Item.rare = ItemRarityID.Pink;
			Item.defense = 14;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type == ModContent.ItemType<SpiritBodyArmor>() && legs.type == ModContent.ItemType<SpiritLeggings>();

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Spirit");

			if (player.statLife < 400)
			{
				player.GetDamage(DamageClass.Melee) += 0.08f;
				player.GetDamage(DamageClass.Ranged) += 0.08f;
				player.GetDamage(DamageClass.Summon) += 0.08f;
				player.GetDamage(DamageClass.Magic) += 0.08f;
			}
			if (player.statLife < 200)
				player.statDefense += 6;
			if (player.statLife < 100)
				player.lifeRegen += 2;
			if (player.statLife < 50)
				player.longInvince = true;
		}

		public override void UpdateEquip(Player player)
		{
			player.statLifeMax2 += 10;
			player.GetDamage(DamageClass.Melee) += 0.12f;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<SpiritBar>(), 15);
			recipe.AddIngredient(ModContent.ItemType<SoulShred>(), 4);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}