using SpiritMod.Items.Material;
using SpiritMod.Tiles.Furniture.Bamboo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Bamboo
{
	public class BambooBarrelSmallItem : ModItem
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Small Stripped Bamboo Barrel");

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 18;
			Item.value = 50;
			Item.maxStack = 999;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<BambooBarrelSmall>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<StrippedBamboo>(), 9);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar);
			recipe.AddTile(TileID.Sawmill);
			recipe.Register();
		}
	}
}