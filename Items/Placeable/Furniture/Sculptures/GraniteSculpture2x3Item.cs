using SpiritMod.Tiles.Furniture.Sculptures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Sculptures
{
	[Sacrifice(1)]
	public class GraniteSculpture2x3Item : ModItem
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Wavy Granite Sculpture");

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 34;
			Item.value = 150;
			Item.maxStack = 99;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<GraniteSculpture2x3>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.GraniteBlock, 50);
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.Register();
		}
	}
}