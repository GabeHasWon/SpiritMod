using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.AceCardsSet
{
	public class FourOfAKind : AccessoryItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Four of a Kind");
			Tooltip.SetDefault("Critical hits deal extra damage\nCritical hits have a chance to drop a diamond ace or heart\nCritical kills drop money");
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<AceOfClubs>());
            recipe.AddIngredient(ModContent.ItemType<AceOfHearts>());
            recipe.AddIngredient(ModContent.ItemType<AceOfSpades>());
			recipe.AddIngredient(ModContent.ItemType<AceOfDiamonds>());
            recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}
}
