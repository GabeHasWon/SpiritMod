using SpiritMod.Buffs.Potion;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Potion
{
	public class DoubleJumpPotion : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Zephyr Potion");
			Tooltip.SetDefault("Temporarily grants a double jump");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 34;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = 30;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = Item.useAnimation = 20;
			Item.consumable = true;
			Item.autoReuse = false;
			Item.buffType = ModContent.BuffType<DoubleJumpPotionBuff>();
			Item.buffTime = 10800;
			Item.UseSound = SoundID.Item3;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<ByBiome.Forest.Placeable.Decorative.CloudstalkItem>(), 1);
			recipe.AddIngredient(ItemID.Cloud, 5);
			recipe.AddIngredient(ItemID.Feather, 1);
			recipe.AddTile(TileID.Bottles);
			recipe.Register();
		}
	}
}
