using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Bullet;
using SpiritMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.TerraGunTree
{
	public class ShadowShot : ModItem, ITimerItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shadeblaster");
			// Tooltip.SetDefault("Converts regular bullets into vile bullets\nRight-click to shoot out a cursed tracker that sticks to enemies\nVile bullets home onto tracked enemies");
		}

		public override void SetDefaults()
		{
			Item.damage = 22;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 50;
			Item.height = 38;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 2;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 4, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<VileBullet>();
			Item.shootSpeed = 10f;
			Item.useAmmo = AmmoID.Bullet;
			Item.crit = 6;
		}

		public override bool AltFunctionUse(Player player) => player.ItemTimer<ShadowShot>() <= 0;

		public override Vector2? HoldoutOffset() => new Vector2(-10, -4);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (!player.IsUsingAlt())
				velocity = velocity.RotatedByRandom(.0785f);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 45f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			if (player.IsUsingAlt())
			{
				player.SetItemTimer<ShadowShot>(300);
				Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<ShadowShotTracker>(), Item.damage / 3, knockback, player.whoAmI);
			}
			else
			{
				if (type == ProjectileID.Bullet) type = ModContent.ProjectileType<VileBullet>();
				Projectile.NewProjectile(source, position, velocity, type, Item.damage, knockback, player.whoAmI);
			}
			return false;
		}

		public override bool? UseItem(Player player)
		{
			if (player.IsUsingAlt())
				SoundEngine.PlaySound(SoundID.Item94);
			else
				SoundEngine.PlaySound(SoundID.Item11);

			return null;
		}

		public override void HoldItem(Player player)
		{
			if (player.ItemTimer<ShadowShot>() == 1)
			{
				if (player.whoAmI == Main.myPlayer)
					SoundEngine.PlaySound(SoundID.MaxMana);

				for (int index1 = 0; index1 < 5; ++index1)
				{
					int index2 = Dust.NewDust(player.position, player.width, player.height, DustID.CursedTorch, 0.0f, 0.0f, (int)byte.MaxValue, new Color(), (float)Main.rand.Next(20, 26) * 0.1f);
					Main.dust[index2].noLight = false;
					Main.dust[index2].noGravity = true;
					Main.dust[index2].velocity *= 0.5f;
				}
			}
		}

		public int TimerCount() => 1;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Boomstick);
			recipe.AddIngredient(ItemID.Musket);
			recipe.AddIngredient(ItemID.Handgun, 1);
			recipe.AddIngredient(ModContent.ItemType<CoilSet.CoilPistol>(), 1);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}