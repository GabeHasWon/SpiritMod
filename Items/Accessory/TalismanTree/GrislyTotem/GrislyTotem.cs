using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.BloodcourtSet;

namespace SpiritMod.Items.Accessory.TalismanTree.GrislyTotem
{
	public class GrislyTotem : AccessoryItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Grisly Talisman");
			Tooltip.SetDefault("Getting hit dislodges a chunk of meat that can be picked up to recover a portion of your lost health\nThis pickup dissapears after a few seconds");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 2, 0, 0);
		}
		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe(1);
			modRecipe.AddIngredient(ItemID.Vertebrae, 8);
			modRecipe.AddIngredient(ModContent.ItemType<DreamstrideEssence>(), 4);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}
}
