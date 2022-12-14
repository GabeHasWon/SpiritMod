using Microsoft.Xna.Framework;
using SpiritMod.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace SpiritMod.Items.Sets.SpiritBiomeDrops
{
	public class SoulWeaver : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Weaver");
			Tooltip.SetDefault("Shoots out multiple Soul Shards in a spread");

		}


		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.useTurn = false;
			Item.autoReuse = true;
			Item.value = Item.sellPrice(0, 1, 60, 0);
			Item.value = Item.buyPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.damage = 38;
			Item.mana = 5;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 11;
			Item.useAnimation = 11;
			Item.UseSound = SoundID.Item9;
			Item.knockBack = 2;
			Item.DamageType = DamageClass.Magic;
			Item.channel = true;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<SoulShard>();
			Item.shootSpeed = 12f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			type = ModContent.ProjectileType<SoulShard>();
			float spread = 30 * 0.0174f;//45 degrees converted to radians
			float baseSpeed = (float)velocity.Length();
			double baseAngle = Math.Atan2(velocity.X, velocity.Y);
			double randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
			velocity.X = baseSpeed * (float)Math.Sin(randomAngle);
			velocity.Y = baseSpeed * (float)Math.Cos(randomAngle);
			return true;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}
	}
}