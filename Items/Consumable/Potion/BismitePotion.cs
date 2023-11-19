using SpiritMod.Buffs.Potion;
using SpiritMod.Items.Sets.BismiteSet;
using SpiritMod.Items.Sets.BriarDrops;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Potion
{
	[Sacrifice(20)]
	public class BismitePotion : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = Item.useAnimation = 20;
			Item.consumable = true;
			Item.autoReuse = false;
			Item.buffType = ModContent.BuffType<BismitePotionBuff>();
			Item.buffTime = 21600;
			Item.UseSound = SoundID.Item3;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BismiteCrystal>(), 1);
			recipe.AddIngredient(ModContent.ItemType<ReachFishingCatch>(), 1);
			recipe.AddIngredient(ItemID.Waterleaf, 1);
			recipe.AddIngredient(ItemID.BottledWater, 1);
			recipe.AddTile(TileID.Bottles);
			recipe.Register();
		}
	}
}
