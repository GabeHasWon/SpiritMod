using SpiritMod.Tiles.Furniture.Ocean;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.ByBiome.Ocean.Placeable;

[Sacrifice(1)]
public class BuoyItem : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Buoy>());
		Item.value = Item.buyPrice(0, 0, 0, 60);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddRecipeGroup(RecipeGroupID.IronBar, 3)
			.AddIngredient(ItemID.Wire, 5)
			.AddIngredient(ItemID.Glass, 5)
			.AddTile(TileID.Anvils)
			.Register();
	}
}