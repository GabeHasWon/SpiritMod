using SpiritMod.Projectiles.Held;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BismiteSet
{
	public class BismiteSpear : ModItem
	{
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.width = 24;
			Item.height = 24;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.useAnimation = 32;
			Item.useTime = 32;
			Item.shootSpeed = 3.8f;
			Item.knockBack = 4f;
			Item.damage = 11;
			Item.value = Item.sellPrice(0, 0, 10, 0);
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<BismiteSpearProj>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<BismiteCrystal>(), 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
