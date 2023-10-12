using SpiritMod.Buffs.Potion;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Potion
{
	[Sacrifice(20)]
	public class FlightPotion : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 26;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = 30;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = Item.useAnimation = 20;
			Item.consumable = true;
			Item.autoReuse = false;
			Item.buffType = ModContent.BuffType<FlightPotionBuff>();
			Item.buffTime = 14400;
			Item.UseSound = SoundID.Item3;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BottledWater, 1);
			recipe.AddIngredient(ModContent.ItemType<ByBiome.Forest.Placeable.Decorative.CloudstalkItem>(), 1);
			recipe.AddIngredient(ItemID.SoulofFlight, 5);
			recipe.AddIngredient(ItemID.Damselfish, 1);
			recipe.AddTile(TileID.Bottles);
			recipe.Register();
		}
	}
}
