using SpiritMod.Items.BossLoot.DuskingDrops;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.DuskingDrops.DuskArmor
{
	[AutoloadEquip(EquipType.Legs)]
	public class DuskLeggings : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dusk Leggings");
			// Tooltip.SetDefault("Increases critical strike chance by 7%");

		}
		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 30;
			Item.value = 40000;
			Item.rare = ItemRarityID.Pink;
			Item.defense = 14;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetCritChance(DamageClass.Magic) += 7;
			player.GetCritChance(DamageClass.Ranged) += 7;
			player.GetCritChance(DamageClass.Melee) += 7;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<DuskStone>(), 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}