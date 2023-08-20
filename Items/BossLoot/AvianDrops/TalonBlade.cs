using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Sword;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AvianDrops
{
	public class TalonBlade : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 30;
			Item.DamageType = DamageClass.Melee;
			Item.width = 34;
			Item.height = 40;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<BoneFeather>();
			Item.shootSpeed = 7f;
			Item.crit = 6;
			Item.autoReuse = true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			SpawnFeather(player, source);
			return false;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(18))
				SpawnFeather(player, player.GetSource_ItemUse(Item));
		}

		private void SpawnFeather(Player player, IEntitySource source)
		{
			Vector2 velocity = Vector2.UnitX * Main.rand.NextFloat(0.5f, 1.0f) * Item.shootSpeed * player.direction;
			Projectile.NewProjectile(source, player.MountedCenter, velocity, Item.shoot, Item.damage, Item.knockBack, player.whoAmI, Main.rand.Next(-20, 0), Main.rand.Next(30, 100));
		}
	}
}

