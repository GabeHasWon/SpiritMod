using SpiritMod.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AcidBenchTile = SpiritMod.Tiles.Furniture.Acid.AcidBenchTile;

namespace SpiritMod.Items.Placeable.Furniture.Acid
{
	[Sacrifice(1)]
	public class AcidBench : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 28;
			Item.value = 200;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<AcidBenchTile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<AcidBrick>(), 14);
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.Register();
		}
	}
}