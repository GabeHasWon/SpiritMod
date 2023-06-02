using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Fish
{
	[Sacrifice(5)]
	public class CrystalFish : FoodItem
	{
		internal override Point Size => new(48, 34);
		public override void StaticDefaults()
		{
			DisplayName.SetDefault("Crystallized Salmon");
			Tooltip.SetDefault("Minor improvements to all stats\nBoosts magic power");
		}

		public override bool CanUseItem(Player player)
		{
			player.AddBuff(BuffID.MagicPower, 9860);
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe(1);
			recipe1.AddIngredient(ModContent.ItemType<RawFish>(), 1);
			recipe1.AddIngredient(ItemID.CrystalShard, 1);
			recipe1.AddTile(TileID.CookingPots);
			recipe1.Register();
		}
	}
}
