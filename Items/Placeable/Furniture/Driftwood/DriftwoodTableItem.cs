using SpiritMod.Items.Placeable.Tiles;
using SpiritMod.Items.Sets.FloatingItems.Driftwood;
using SpiritMod.Tiles.Furniture;
using SpiritMod.Tiles.Furniture.Driftwood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Driftwood
{
	[Sacrifice(1)]
	public class DriftwoodTableItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 25;
			Item.value = 150;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<DriftwoodTable>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}