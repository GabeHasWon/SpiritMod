using SpiritMod.Tiles.Furniture.Bamboo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Bamboo
{
	public class BambooPotItem : ModItem
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Stripped Bamboo Pot");

		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.value = 50;
			Item.maxStack = 999;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<BambooPot>();
		}

		/*public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<StrippedBamboo>(), 5);
			recipe.AddTile(TileID.Sawmill);
			recipe.Register();
		}*/
	}
}