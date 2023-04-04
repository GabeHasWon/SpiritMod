using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.ScarabeusDrops.ChitinArmor
{
	[AutoloadEquip(EquipType.Legs)]
	public class ChitinLeggings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chitin Leggings");
			Tooltip.SetDefault("Increases movement speed by 7%");
		}
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 18;
			Item.value = Item.sellPrice(silver: 10);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 3;
		}
		public override void UpdateEquip(Player player)
		{
			player.moveSpeed *= 1.07f;
			player.maxRunSpeed *= 1.07f;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<Chitin>(), 12);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
