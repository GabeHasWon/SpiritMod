using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.FloranSet
{
	[Sacrifice(25)]
	public class FloranBar : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = 550;
			Item.rare = ItemRarityID.Blue;
			Item.createTile = ModContent.TileType<FloranBarTile>();
			Item.maxStack = Item.CommonMaxStack;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FloranOre>(), 4);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
		}
	}
}