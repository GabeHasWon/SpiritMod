using SpiritMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture
{
	[Sacrifice(1)]
	public class OldTelescope : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 34;
			Item.value = 150;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<OldTelescopeTile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup(RecipeGroupID.Wood, 20);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 3);
			recipe.AddIngredient(ItemID.BlackLens, 1);
			recipe.AddTile(TileID.Sawmill);
			recipe.Register();
		}
	}
}