using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AtlasDrops.PrimalstoneArmor
{
	[AutoloadEquip(EquipType.Body)]
	public class PrimalstoneBreastplate : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 30;
			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.Cyan;
			Item.defense = 19;
		}

		public override void UpdateEquip(Player player)
		{
			player.lifeRegen += 2;
			player.GetCritChance(DamageClass.Melee) += 17;
			player.GetDamage(DamageClass.Melee) += .05f;
			player.GetCritChance(DamageClass.Magic) += 7;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<ArcaneGeyser>(), 14);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}