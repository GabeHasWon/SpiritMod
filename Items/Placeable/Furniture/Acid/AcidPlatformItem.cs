using SpiritMod.Items.Placeable.Tiles;
using SpiritMod.Tiles.Furniture.Acid;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Acid
{
	[Sacrifice(200)]
	public class AcidPlatformItem : ModItem
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
			Item.createTile = ModContent.TileType<AcidPlatform>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(2);
			recipe.AddIngredient(ModContent.ItemType<AcidBrick>());
			recipe.Register();
		}
	}
}