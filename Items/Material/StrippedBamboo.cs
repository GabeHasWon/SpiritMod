using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Material
{
	public class StrippedBamboo : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.value = 1;
			Item.rare = ItemRarityID.White;
			Item.maxStack = 999;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BambooBlock);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
