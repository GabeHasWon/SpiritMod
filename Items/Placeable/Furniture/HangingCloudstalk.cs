using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Tiles.Furniture.Hanging;
using SpiritMod.Items.ByBiome.Forest.Placeable.Decorative;

namespace SpiritMod.Items.Placeable.Furniture
{
	[Sacrifice(1)]
	public class HangingCloudstalk : ModItem
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Hanging Cloudstalk");

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 28;
			Item.value = Item.value = Item.sellPrice(0, 0, 1, 50);
			Item.rare = ItemRarityID.White;
			Item.maxStack = 99;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<HangingCloudstalkTile>(); 
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.PotSuspended);
            recipe.AddIngredient(ModContent.ItemType<CloudstalkItem>());
			recipe.Register();
		}
	}
}