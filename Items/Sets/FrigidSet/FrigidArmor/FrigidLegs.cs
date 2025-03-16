using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.FrigidSet.FrigidArmor;

[AutoloadEquip(EquipType.Legs)]
public class FrigidLegs : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 24;
		Item.value = 1100;
		Item.rare = ItemRarityID.Blue;
		Item.defense = 2;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<FrigidFragment>(), 6).AddTile(TileID.Anvils).Register();
}
