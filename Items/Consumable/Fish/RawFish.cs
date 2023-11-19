using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Fish
{
	public class RawFish : FoodItem
	{
		internal override Point Size => new(34, 22);

		public override bool CanUseItem(Player player)
		{
			player.AddBuff(BuffID.Poisoned, 600);
			return true;
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
}
