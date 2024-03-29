using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SpiritSet.SpiritArmor
{
	[AutoloadEquip(EquipType.Body)]
	public class SpiritBodyArmor : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spirit Body Armor");
			// Tooltip.SetDefault("Increases melee damage and critical strike chance by 10%, as well as reducing damage taken by 10%");
		}

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 30;
			Item.value = 50000;
			Item.rare = ItemRarityID.Pink;
			Item.defense = 18;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(DamageClass.Melee) += 0.10f;
			player.GetCritChance(DamageClass.Melee) += 10;
			player.endurance = .10f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<SpiritBar>(), 20)
				.AddIngredient(ModContent.ItemType<SoulShred>(), 5)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}