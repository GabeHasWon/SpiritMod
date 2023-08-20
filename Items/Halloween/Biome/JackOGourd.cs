using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Halloween.Biome
{
	public class JackOGourd : FoodItem
	{
		internal override Point Size => new(32, 38);
		public override void StaticDefaults()
		{
			// DisplayName.SetDefault("Jack-o-Gourd");
			// Tooltip.SetDefault("Minor improvements to all stats...kinda");
		}

		public override bool? UseItem(Player player)
		{
			player.AddBuff(BuffID.OnFire, 3 * 60 * 60);
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient<TreeGourd>(1).
				AddIngredient(ItemID.Torch, 4).
				Register();

			CreateRecipe(4).
				AddIngredient<TreeGourd>(4).
				AddIngredient(ItemID.LivingFireBlock, 1).
				Register();
		}
	}
}
