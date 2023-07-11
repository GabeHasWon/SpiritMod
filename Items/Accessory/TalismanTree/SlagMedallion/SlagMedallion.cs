using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.SlagSet;

namespace SpiritMod.Items.Accessory.TalismanTree.SlagMedallion
{
	[AutoloadEquip(EquipType.Neck)]
	internal class SlagMedallion : AccessoryItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slag Medallion");
			Tooltip.SetDefault("Getting hit grants a temporary damage buff\nThis buff scales with the damage taken");
		}
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 36;
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 4, 0, 0);
		}
		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ItemID.HellstoneBar, 5);
			modRecipe.AddIngredient(ModContent.ItemType<CarvedRock>(), 8);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}
}
