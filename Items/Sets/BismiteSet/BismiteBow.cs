using Microsoft.Xna.Framework;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace SpiritMod.Items.Sets.BismiteSet
{
	public class BismiteBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bismite Bow");
			// Tooltip.SetDefault("Occasionally causes foes to receive 'Festering Wounds,' which deal more damage to enemies under half health");
		}

		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 20;
			Item.height = 46;
			Item.useTime = 21;
			Item.useAnimation = 21;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.Shuriken;
			Item.useAmmo = AmmoID.Arrow;
			Item.knockBack = 1;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = false;
			Item.shootSpeed = 6.5f;
			Item.crit = 8;
			Item.reuseDelay = 20;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BismiteCrystal>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 18;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
			proj.GetGlobalProjectile<SpiritGlobalProjectile>().shotFromBismiteBow = true;

			int numLoops = 25;
			for (int k = 0; k < numLoops; k++)
			{
				Vector2 offset = Vector2.Normalize(velocity) * 15f;
				Dust dust = Dust.NewDustPerfect(proj.Center + offset, DustID.Plantera_Green);
				dust.velocity = (Vector2.UnitX * (velocity.Length() / 3.5f)).RotatedBy(MathHelper.TwoPi / numLoops * k);
				dust.noGravity = true;
			}
			return false;
		}
	}
}