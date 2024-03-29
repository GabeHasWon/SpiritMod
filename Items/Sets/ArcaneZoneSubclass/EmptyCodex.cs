using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ArcaneZoneSubclass
{
	public class EmptyCodex : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 500;
			Item.rare = ItemRarityID.Green;
		}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Book, 1);
            recipe.AddIngredient(ItemID.FallenStar, 2);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
