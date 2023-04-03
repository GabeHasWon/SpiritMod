using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Clubs.BruteHammer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ClubSubclass
{
	public class BruteHammer : ClubItem
	{
		internal override int MinDamage => 35;
		internal override int MaxDamage => 160;
		internal override float MinKnockback => 5;
		internal override float MaxKnockback => 14;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brute Hammer");
			Tooltip.SetDefault("Spins rapidly around the player, dealing a devastating blow on release");
		}

		public override void Defaults()
		{
			Item.width = 40;
			Item.height = 32;
			Item.UseSound = SoundID.Item1;
			Item.value = Item.sellPrice(0, 0, 60, 0);
			Item.rare = ItemRarityID.Orange;
			Item.shoot = ModContent.ProjectileType<BruteHammerProj>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			StatModifier meleeDMG = player.GetTotalDamage(DamageClass.Melee);
			StatModifier meleeKB = player.GetTotalKnockback(DamageClass.Melee);

			Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
			(proj.ModProjectile as BruteHammerProj).minDamage = (int)meleeDMG.ApplyTo(MinDamage);
			(proj.ModProjectile as BruteHammerProj).maxDamage = (int)meleeDMG.ApplyTo(MaxDamage);
			(proj.ModProjectile as BruteHammerProj).minKnockback = (int)meleeKB.ApplyTo(MinKnockback);
			(proj.ModProjectile as BruteHammerProj).maxKnockback = (int)meleeKB.ApplyTo(MaxKnockback);

			return false;
		}
	}
}