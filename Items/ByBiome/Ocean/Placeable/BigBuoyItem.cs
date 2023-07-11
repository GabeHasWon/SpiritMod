using SpiritMod.Tiles.Furniture.Ocean;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.ByBiome.Ocean.Placeable;

public class BigBuoyItem : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Large Buoy");
		Tooltip.SetDefault("Must be placed atop water");
	}

	public override void SetDefaults() => Item.DefaultToPlaceableTile(ModContent.TileType<BigBuoy>());

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddRecipeGroup(RecipeGroupID.IronBar, 6)
			.AddIngredient(ItemID.Wire, 6)
			.AddIngredient(ItemID.Glass, 6)
			.AddTile(TileID.Anvils)
			.Register();
	}
}