using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Tiles.Block;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Tiles
{
	public class SpiritWoodItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 14;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<SpiritWood>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Walls.SpiritWallItem>(), 4);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();

			Recipe recipe2 = CreateRecipe();
			recipe2.AddIngredient(ModContent.ItemType<SpiritPlatformItem>(), 2);
			recipe2.Register();
		}
	}
}