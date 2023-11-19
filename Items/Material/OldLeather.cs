using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Material
{
	public class OldLeather : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.value = 500;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ItemID.Leather);
			recipe.AddIngredient(this, 2);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
