using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops.InfernonArmor
{
	[AutoloadEquip(EquipType.Body)]
	public class InfernalBreastplate : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.rare = ItemRarityID.Pink;
			Item.value = 62000;
			Item.defense = 3;
		}

		public override void UpdateEquip(Player player)
		{
			player.whipRangeMultiplier *= 1.1f;
			player.GetDamage(DamageClass.Summon) += 0.1f;
			player.maxMinions += 1;
		}

		public override void AddRecipes()
		{
			var recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<InfernalAppendage>(), 16);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}

	}
}
