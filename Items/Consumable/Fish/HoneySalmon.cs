using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Fish
{
	[Sacrifice(5)]
	public class HoneySalmon : FoodItem
	{
		internal override Point Size => new(52, 38);
		public override void StaticDefaults()
		{
			// DisplayName.SetDefault("Honey-Glazed Salmon");
			// Tooltip.SetDefault("Minor improvements to all stats\nBoosts life regeneration");
		}

		public override bool CanUseItem(Player player)
		{
			player.AddBuff(48, 1800);
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe(1);
			recipe1.AddIngredient(ModContent.ItemType<RawFish>(), 2);
			recipe1.AddIngredient(ItemID.BottledHoney, 1);
			recipe1.AddTile(TileID.CookingPots);
			recipe1.Register();
		}
	}
}
