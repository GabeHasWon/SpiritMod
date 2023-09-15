using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.BloodcourtSet;

namespace SpiritMod.Items.Accessory.TalismanTree.GrislyTongue
{
	public class GrislyTongue : AccessoryItem
	{

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