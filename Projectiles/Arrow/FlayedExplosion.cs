using SpiritMod.Buffs;
using SpiritMod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Arrow
{
	public class FlayedExplosion : ModProjectile
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Flayed Explosion");

		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.alpha = 255;
			Projectile.timeLeft = 1;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
		}

		public override void AI()
		{
			ProjectileExtras.Explode(Projectile.whoAmI, 260, 260,
			delegate
			{
				for (int i = 0; i < 60; i++)
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<NightmareDust>(), 0f, -2f, 0, default, 1.1f);
					dust.noGravity = true;
					dust.scale = 1.5f;

					dust.position.X += (Main.rand.Next(-30, 31) / 20) - 1.5f;
					dust.position.Y += (Main.rand.Next(-30, 31) / 20) - 1.5f;

					if (dust.position != Projectile.Center)
						dust.velocity = Projectile.DirectionTo(dust.position) * 9f;
				}
			});
			Projectile.active = false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.NextBool(4))
				target.AddBuff(ModContent.BuffType<SurgingAnguish>(), 200, true);
		}
	}
}