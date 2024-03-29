using SpiritMod.Tiles.Furniture.Sculptures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Sculptures
{
	[Sacrifice(1)]
	public class MarbleSculpture1x2Item : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 34;
			Item.value = 150;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<MarbleSculpture1x2>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.MarbleBlock, 25);
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.Register();
		}
	}
}