using SpiritMod.Tiles.Ambient;
using SpiritMod.Items.Sets.MarbleSet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture
{
	[Sacrifice(1)]
	public class MarbleObeliskItem : ModItem
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
			Item.createTile = ModContent.TileType<MarbleObelisk>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.MarbleBlock, 15);
			recipe.AddIngredient(ModContent.ItemType<MarbleChunk>(), 4);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}