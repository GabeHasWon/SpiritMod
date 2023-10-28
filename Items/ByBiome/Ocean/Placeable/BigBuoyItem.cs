using SpiritMod.Tiles.Furniture.Ocean;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.ByBiome.Ocean.Placeable;

[Sacrifice(1)]
public class BigBuoyItem : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<BigBuoy>());
		Item.value = Item.buyPrice(copper: 80);
	}

	public override bool CanUseItem(Player player) => BigBuoy.CanPlaceStatic(Player.tileTargetX, Player.tileTargetY);

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddRecipeGroup(RecipeGroupID.IronBar, 8)
			.AddIngredient(ItemID.Wire, 7)
			.AddIngredient(ItemID.Glass, 7)
			.AddTile(TileID.Anvils)
			.Register();
	}
}