using Microsoft.Xna.Framework;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.HuskstalkSet
{
	public class HuskstalkBow : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 7;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 20;
			Item.height = 38;
			Item.useTime = 26;
			Item.useAnimation = 26;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.Shuriken;
			Item.useAmmo = AmmoID.Arrow;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item5;
			Item.value = Item.sellPrice(0, 0, 12, 0);
			Item.autoReuse = false;
			Item.shootSpeed = 7f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI).GetGlobalProjectile<SpiritGlobalProjectile>().witherLeaf = true;
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<AncientBark>(), 6);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}