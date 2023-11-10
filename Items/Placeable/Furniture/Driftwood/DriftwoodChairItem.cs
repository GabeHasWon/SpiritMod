using SpiritMod.Items.Material;
using SpiritMod.Items.Sets.FloatingItems.Driftwood;
using SpiritMod.Tiles.Furniture.Driftwood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Driftwood
{
	public class DriftwoodChairItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 18;
			Item.value = 50;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<DriftwoodChair>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 4);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}