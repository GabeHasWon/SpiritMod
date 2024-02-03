using SpiritMod.Items.Material;
using SpiritMod.Tiles.Furniture.Bamboo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Bamboo
{
	[Sacrifice(1)]
	public class BambooBenchItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 22;
			Item.value = 50;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<BambooBench>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<StrippedBamboo>(), 14);
			recipe.AddTile(TileID.Sawmill);
			recipe.Register();
		}
	}
}