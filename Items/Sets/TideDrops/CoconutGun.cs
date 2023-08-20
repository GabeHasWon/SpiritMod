using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Bullet;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.TideDrops
{
	public class CoconutGun : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Coconut Gun");
			// Tooltip.SetDefault("'Fires in Spurts' \n'If it shoots ya, it's gonna hurt!'");
		}

		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 65;
			Item.height = 21;
			Item.useTime = 46;
			Item.useAnimation = 46;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 7.5f;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<CoconutSpurt>();
			Item.shootSpeed = 10f;
			Item.crit = 2;
			Item.UseSound = SoundID.Item61;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-7, 0);

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			for (int i = 0; i < 3; i++)
			{
				Vector2 randomVel = (velocity * Main.rand.NextFloat(0.8f, 1.0f)).RotatedByRandom(.3f);
				Projectile.NewProjectile(source, position, randomVel, type, damage, knockback, player.whoAmI);
			}

			return false;
		}
	}
}
