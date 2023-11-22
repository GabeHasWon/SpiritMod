using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.BossLoot.ScarabeusDrops;

namespace SpiritMod.Items.Accessory.TalismanTree.GildedScarab
{
	internal class GildedScarab : AccessoryItem
	{
		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 32;
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 1, 0, 0);
		}
		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ItemID.PlatinumBar, 5);
			modRecipe.AddIngredient(ModContent.ItemType<Chitin>(), 8);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();

			modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ItemID.GoldBar, 5);
			modRecipe.AddIngredient(ModContent.ItemType<Chitin>(), 8);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}
}