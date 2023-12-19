using SpiritMod.Items.Sets.FloatingItems.Driftwood;
using SpiritMod.Tiles.Furniture.Driftwood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Driftwood
{
	[Sacrifice(200)]
	public class DriftwoodPlatformItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 8;
			Item.height = 10;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<DriftwoodPlatform>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(2);
			recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>());
			recipe.Register();
		}
	}
}