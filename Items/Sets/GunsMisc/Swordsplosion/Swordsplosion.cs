using Microsoft.Xna.Framework;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Swordsplosion
{
	public class Swordsplosion : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Swordsplosion");
			// Tooltip.SetDefault("Shoots out a barrage of swords\nProjectiles fired count both as melee and ranged projectiles");
		}

		public override void SetDefaults()
		{
			Item.damage = 76;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 60;
			Item.height = 26;
			Item.useTime = Item.useAnimation = 19;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 6;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item36;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SwordBarrage>();
			Item.shootSpeed = 10f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 55f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			for (int i = 0; i < 3; i++)
			{
				Vector2 randomVel = velocity.RotatedByRandom(.785f);
				Projectile.NewProjectile(source, position, randomVel, ModContent.ProjectileType<SwordBarrage>(), Item.damage, knockback, Item.playerIndexTheItemIsReservedFor, 0, 0);
			}
			return false;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
	}
}