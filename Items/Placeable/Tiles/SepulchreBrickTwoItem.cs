using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.World.Sepulchre;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Tiles
{
	public class SepulchreBrickTwoItem : ModItem
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
			Item.createTile = ModContent.TileType<SepulchreBrickTwo>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(10);
			recipe.AddIngredient(ItemID.GrayBrick, 5);
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.Register();

			Recipe recipe2 = CreateRecipe();
			recipe2.AddIngredient(ModContent.ItemType<SepulchrePlatformItem>(), 2);
			recipe2.Register();
		}
	}
}