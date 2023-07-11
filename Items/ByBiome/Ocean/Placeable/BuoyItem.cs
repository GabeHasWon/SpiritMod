using SpiritMod.Tiles.Furniture.Ocean;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.ByBiome.Ocean.Placeable;

public class BuoyItem : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Small Buoy");
		Tooltip.SetDefault("Must be placed atop water");
	}

	public override void SetDefaults() => Item.DefaultToPlaceableTile(ModContent.TileType<Buoy>());

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddRecipeGroup(RecipeGroupID.IronBar, 2)
			.AddIngredient(ItemID.Wire, 2)
			.AddIngredient(ItemID.Glass, 2)
			.AddTile(TileID.Anvils)
			.Register();
	}
}