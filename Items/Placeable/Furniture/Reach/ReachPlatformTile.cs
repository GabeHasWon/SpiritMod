using SpiritMod.Items.Sets.HuskstalkSet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReachPlatform = SpiritMod.Tiles.Furniture.Reach.ReachPlatform;

namespace SpiritMod.Items.Placeable.Furniture.Reach
{
	[Sacrifice(200)]
	public class ReachPlatformTile : ModItem
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
			Item.createTile = ModContent.TileType<ReachPlatform>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(2);
			recipe.AddIngredient(ModContent.ItemType<AncientBark>());
			recipe.Register();
		}
	}
}