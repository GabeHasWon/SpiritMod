using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.SilkArmor
{
	[AutoloadEquip(EquipType.Head, EquipType.Front)]
	public class Earrings : ModItem
	{
		public override void SetStaticDefaults() => ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.value = 7500;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddRecipeGroup("SpiritMod:GoldBars", 2);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}