using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Bloater
{
	public class VomitProj : ModProjectile
	{
		private readonly int timeLeftMax = 22;

		public override void SetDefaults()
		{
			Projectile.width = 6;
			Projectile.height = 6;
			AIType = ProjectileID.Flames;
			Projectile.alpha = 255;
			Projectile.timeLeft = timeLeftMax;
			Projectile.penetrate = 4;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			Projectile.rotation += 0.1f;
			for (int i = 0; i < 4; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				dust.scale = Main.rand.NextFloat(.8f, 1.5f) * (1f - ((float)Projectile.timeLeft / timeLeftMax) + .3f);
				dust.noGravity = true;
				if (Main.rand.NextBool(4))
					dust.fadeIn = 1f;
			}
		}
	}
}
