using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Fish
{
	[Sacrifice(5)]
	public class FishChips : FoodItem
	{
		internal override Point Size => new(42, 30);
		public override void StaticDefaults()
		{
			DisplayName.SetDefault("Fish n' Chips");
			Tooltip.SetDefault("Minor improvements to all stats\nMakes you sluggish");
		}

		public override bool CanUseItem(Player player)
		{
			player.AddBuff(ModContent.BuffType<CouchPotato>(), 3600);
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe(1);
			recipe1.AddIngredient(ModContent.ItemType<RawFish>(), 5);
			recipe1.AddTile(TileID.CookingPots);
			recipe1.Register();
		}
	}
}
