using SpiritMod.Tiles.Furniture;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture
{
	[Sacrifice(1)]
	public class GiantAnglerStatue : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 34;
			Item.value = 5000;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.buyPrice(gold: 3);
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<GiantAnglerStatueTile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FishCrate>(), 3);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 20);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
		}
	}
}