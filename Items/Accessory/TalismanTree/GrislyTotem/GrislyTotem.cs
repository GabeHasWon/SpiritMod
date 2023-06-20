using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.TalismanTree.GrislyTotem
{
	public class GrislyTotem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Grisly Totem");
			Tooltip.SetDefault("Getting hit dislodges a chunk of meat that can be picked up to recover a portion of your lost health.\nThis pickup dissapears after a few seconds.");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item11;
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 0, 30, 0);
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetSpiritPlayer().grislyTotem = true;


		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe(1);
			modRecipe.AddIngredient(ItemID.Vertebrae, 8);
			modRecipe.AddIngredient(ItemID.TissueSample, 5);
			modRecipe.AddIngredient(ItemID.Chain, 3);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}
}
