using SpiritMod.Items.Sets.BismiteSet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Leather
{
	[AutoloadEquip(EquipType.Shield)]
	public class BismiteShield : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.defense = 1;
            Item.value = Item.sellPrice(0, 0, 60, 0);
            Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetSpiritPlayer().bismiteShield = true;
		
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<LeatherShield>(), 1);
			recipe.AddIngredient(ModContent.ItemType<BismiteCrystal>(), 6);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
