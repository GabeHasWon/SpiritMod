using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.CooldownItem;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops
{
	public class InfernalStaff : ModItem, ICooldownItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seal of Torment");
			Tooltip.SetDefault("Shoots three exploding, homing, fiery souls\n3 second cooldown");
		}

		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 42;
			Item.rare = ItemRarityID.Pink;
			Item.mana = 12;
			Item.damage = 55;
			Item.knockBack = 5F;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.value = Item.sellPrice(0, 2, 50, 0);
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<FireSoul>();
			Item.shootSpeed = 12f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			CooldownGItem.GetCooldown(Type, player, 180);

			for (int i = 0; i < 3; i++)
			{
				velocity = (velocity * Main.rand.NextFloat(0.75f, 1.0f)).RotatedByRandom(0.5f);
				Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
			}
			return false;
		}

		public override bool CanUseItem(Player player) => CooldownGItem.GetCooldown(Type, player) == 0;
	}
}