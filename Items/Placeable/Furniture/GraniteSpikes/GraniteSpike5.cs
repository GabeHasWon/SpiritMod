using SpiritMod.Tiles.Ambient.GraniteSpike;
using SpiritMod.Items.Sets.GraniteSet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Tiles.Furniture;

namespace SpiritMod.Items.Placeable.Furniture.GraniteSpikes
{
	public class GraniteSpike5 : ModItem
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
			Item.createTile = ModContent.TileType<Spike5Tile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.GraniteBlock, 10);
			recipe.AddIngredient(ModContent.ItemType<GraniteChunk>(), 1);
			recipe.AddTile(ModContent.TileType<ForagerTableTile>());
			recipe.Register();
		}
	}
}