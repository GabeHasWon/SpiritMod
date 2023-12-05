using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Leather
{
	[AutoloadEquip(EquipType.Shield)]
	public class Strikeshield : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.defense = 1;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetSpiritPlayer().strikeshield = true;
		
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<LeatherShield>(), 1);
            recipe.AddIngredient(ItemID.ManaCrystal, 1);
            recipe.AddIngredient(ModContent.ItemType<Placeable.Tiles.BlastStoneItem>(), 10);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
