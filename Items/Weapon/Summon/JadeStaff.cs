using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Summon.Dragon;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Summon
{
	public class JadeStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Staff of the Jade Dragon");
			Tooltip.SetDefault("Summons two revolving ethereal dragons");
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 23;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 36;
			Item.width = 44;
			Item.height = 48;
			Item.useTime = 80;
			Item.useAnimation = 80;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 2.25f;
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.NPCHit56;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<DragonHeadOne>();
			Item.shootSpeed = 6f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			int dragonLength = 8;
			int offset = -(32 * Math.Sign(velocity.X));

			int latestprojectile = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DragonHeadOne>(), damage, knockback, player.whoAmI, velocity.X, velocity.Y); //bottom
			for (int i = 0; i < dragonLength; ++i)
			{
				latestprojectile = Projectile.NewProjectile(source, position.X + (i * offset), position.Y, 0, 0, ModContent.ProjectileType<DragonBodyOne>(), damage, 0, player.whoAmI, 0, latestprojectile);
			}
			latestprojectile = Projectile.NewProjectile(source, position.X + (dragonLength * offset), position.Y, 0, 0, ModContent.ProjectileType<DragonTailOne>(), damage, 0, player.whoAmI, 0, latestprojectile);

			latestprojectile = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DragonHeadTwo>(), damage, knockback, player.whoAmI, velocity.X, velocity.Y); //bottom
			for (int j = 0; j < dragonLength; ++j)
			{
				latestprojectile = Projectile.NewProjectile(source, position.X + (j * offset), position.Y, 0, 0, ModContent.ProjectileType<DragonBodyTwo>(), damage, 0, player.whoAmI, 0, latestprojectile);
			}
			latestprojectile = Projectile.NewProjectile(source, position.X + (dragonLength * offset), position.Y, 0, 0, ModContent.ProjectileType<DragonTailTwo>(), damage, 0, player.whoAmI, 0, latestprojectile);
			return true;
		}
	}
}