
using SpiritMod.Items.Material;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops.InfernonArmor
{
	[AutoloadEquip(EquipType.Legs)]
	public class InfernalGreaves : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.rare = ItemRarityID.Pink;
			Item.value = 42000;

			Item.defense = 1;
		}
		public override void UpdateEquip(Player player)
		{
			player.GetDamage(DamageClass.Summon) += 0.1f;
			player.maxMinions += 1;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<InfernalAppendage>(), 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}

	}
}
