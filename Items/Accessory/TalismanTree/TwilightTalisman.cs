using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.TalismanTree
{
	public class TwilightTalisman : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Twilight Talisman");
			Tooltip.SetDefault("Increases melee speed by 5%\nIncreases damage reduction and damage dealt by 5%\nIncreases critical strike chance by 4%\nAttacks have a small chance of inflicting Shadowflame");
		}

		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 24;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item11;
			Item.accessory = true;
			Item.value = Item.buyPrice(0, 2, 30, 0);
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetSpiritPlayer().twilightTalisman = true;

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe(1);
			modRecipe.AddIngredient(ModContent.ItemType<AmazonianCharm>(), 1);
			modRecipe.AddIngredient(ModContent.ItemType<HellMedallion>(), 1);
			modRecipe.AddIngredient(ModContent.ItemType<VileCharm>(), 1);
			modRecipe.AddIngredient(ModContent.ItemType<MarrowPendant>(), 1);
			modRecipe.AddTile(TileID.DemonAltar);
			modRecipe.Register();
		}
	}
}
