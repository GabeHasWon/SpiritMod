using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Fish;

public class RawFish : FoodItem
{
	internal override Point Size => new(34, 22);

	public override void Defaults()
	{
		base.Defaults();

		Item.buffType = BuffID.Poisoned;
	}

	public override void AddRecipes()
	{
		Recipe recipe1 = Recipe.Create(ItemID.CookedFish, 1);
		recipe1.AddIngredient(ModContent.ItemType<RawFish>(), 1);
		recipe1.AddTile(TileID.CookingPots);
		recipe1.Register();

		Recipe recipe2 = Recipe.Create(ItemID.Sashimi, 1);
		recipe2.AddIngredient(ModContent.ItemType<RawFish>(), 1);
		recipe2.AddTile(TileID.WorkBenches);
		recipe2.Register();
	}
}
