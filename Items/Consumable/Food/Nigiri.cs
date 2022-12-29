using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Consumable.Fish;
using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Consumable.Food
{
	public class Nigiri : FoodItem
	{
		internal override Point Size => new(44, 28);
		public override void StaticDefaults() => Tooltip.SetDefault("Minor improvements to all stats\nProvides free movement in water\n'The perfect cut'");

		public override bool CanUseItem(Player player)
		{
			player.AddBuff(BuffID.Flipper, 3600);
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe(1);
			recipe1.AddIngredient(ModContent.ItemType<Sets.FloatingItems.Kelp>(), 7);
			recipe1.AddIngredient(ModContent.ItemType<RawFish>(), 1);
			recipe1.AddTile(TileID.CookingPots);
			recipe1.Register();
		}
	}
}
