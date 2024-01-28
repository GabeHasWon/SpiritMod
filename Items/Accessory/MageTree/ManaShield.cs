using Terraria;
using Terraria.ID;
using SpiritMod.Items.Accessory.Leather;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.MageTree
{
    [AutoloadEquip(EquipType.Shield)]
    public class ManaShield : AccessoryItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.value = Item.sellPrice(0, 0, 80, 0);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void SafeUpdateAccessory(Player player, bool hideVisual) => player.statManaMax2 += 20;

		public override void AddRecipes()
		{
			var recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<LeatherShield>());
			recipe.AddIngredient(ItemID.FallenStar, 3);
            recipe.AddRecipeGroup("SpiritMod:PHMEvilMaterial", 2);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
