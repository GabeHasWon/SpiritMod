using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Spirit
{
	class SpiritScythe : ModProjectile
	{
		private ref float Timer => ref Projectile.ai[0];
		private ref float InitialMagnitude => ref Projectile.ai[1];

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.penetrate = 2;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 300;
			Projectile.height = 28;
			Projectile.width = 28;
			Projectile.extraUpdates = 1;

			AIType = ProjectileID.Bullet;
		}

		public override void AI()
		{
			Projectile.rotation += 0.2f;

			if (Timer % 2 == 0)
			{
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Flare_Blue, Projectile.velocity.X, Projectile.velocity.Y);
				int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Flare_Blue, Projectile.velocity.X, Projectile.velocity.Y);
				Main.dust[dust].noGravity = true;
				Main.dust[dust2].noGravity = true;
				Main.dust[dust2].velocity *= 0f;
				Main.dust[dust2].velocity *= 0f;
				Main.dust[dust2].scale = 1.2f;
				Main.dust[dust].scale = 1.2f;
			}

			Timer++;

			if (Timer == 1)
				InitialMagnitude = Projectile.velocity.Length();

			if (Timer <= 20f)
				Projectile.velocity *= 0.98f;
			else if (Projectile.velocity.Length() < InitialMagnitude * 3)
				Projectile.velocity *= 1.01f;
				
		}

		public override void OnKill(int timeLeft)
		{
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Flare_Blue, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
			int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Flare_Blue, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust2].noGravity = true;
			Main.dust[dust2].velocity *= 0f;
			Main.dust[dust2].velocity *= 0f;
			Main.dust[dust2].scale = 1.2f;
			Main.dust[dust].scale = 1.2f;
		}
	}
}
