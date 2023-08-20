using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Fish
{
	public class SteamedMussels : FoodItem
	{
		internal override Point Size => new(46, 22);
		public override void StaticDefaults()
		{
			// DisplayName.SetDefault("Steamed Mussels");
			// Tooltip.SetDefault("Minor improvements to all stats");
		}

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe(1);
			recipe1.AddIngredient(ModContent.ItemType<Tiles.Ambient.Ocean.MusselItem>(), 3);
			recipe1.AddIngredient(ModContent.ItemType<Sets.FloatingItems.Kelp>(), 1);
			recipe1.AddTile(TileID.CookingPots);
			recipe1.Register();
		}
	}
}
