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

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			velocity = velocity.RotatedBy(MathHelper.ToRadians(45));
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
	}
}