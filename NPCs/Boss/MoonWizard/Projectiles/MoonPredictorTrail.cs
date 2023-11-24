using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Boss.MoonWizard.Projectiles
{
	public class MoonPredictorTrail : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.timeLeft = 50;
			Projectile.damage = 13;
			Projectile.alpha = 255;
			Projectile.width = Projectile.height = 32;
		}

		public override void AI() => Dust.NewDustPerfect(Projectile.Center, 206);
	}
}