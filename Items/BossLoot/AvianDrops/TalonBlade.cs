using Microsoft.Xna.Framework;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AvianDrops
{
	public class TalonBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Talon Blade");
			Tooltip.SetDefault("Launches fossilized feathers");
		}

		int charger;

		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.DamageType = DamageClass.Melee;
			Item.width = 34;
			Item.height = 40;
			Item.useTime = 55;
			Item.useAnimation = 29;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<BoneFeatherSwordProj>();
			Item.shootSpeed = 9f;
			Item.crit = 6;
			Item.autoReuse = true;
		}
		//public override bool OnlyShootOnSwing => true;
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			SoundEngine.PlaySound(SoundID.Item8, position);
			charger++;
			if (charger >= 5)
			{
				SoundEngine.PlaySound(SoundID.Item20, position);
				int p = Projectile.NewProjectile(source, position.X - 8, position.Y + 8, velocity.X + ((float)Main.rand.Next(-230, 230) / 100), velocity.Y + ((float)Main.rand.Next(-230, 230) / 100), ModContent.ProjectileType<GiantFeather>(), damage, knockback, player.whoAmI, 0f, 0f);
				Main.projectile[p].DamageType = DamageClass.Melee;
				charger = 0;
			}
			return true;
		}
	}
}

