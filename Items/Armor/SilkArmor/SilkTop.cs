using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.SilkArmor
{
	[AutoloadEquip(EquipType.Body)]
	public class SilkTop : ModItem
	{
		public override void Load()
		{
			if (Main.netMode == NetmodeID.Server)
				return;
			EquipLoader.AddEquipTexture(Mod, "SpiritMod/Items/Armor/SilkArmor/SilkTopFemale_Body", EquipType.Body, null, "AltTop");
		}

		public override void SetStaticDefaults() => DisplayName.SetDefault("Desert Top");

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 30;
			Item.value = 12500;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Silk, 5);
			recipe.AddRecipeGroup("SpiritMod:GoldBars", 2);
			recipe.AddIngredient(ItemID.FallenStar, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}