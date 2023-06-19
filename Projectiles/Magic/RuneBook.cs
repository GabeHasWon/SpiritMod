using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic
{
	public class RuneBook : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rune Book");
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 28;
			Projectile.height = 32;
			Projectile.timeLeft = 100;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			if (++Projectile.frameCounter >= 6)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
			}
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

			for (int k = 0; k < 15; k++)
				Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);

			if (Projectile.owner == Main.myPlayer)
				for (int h = 0; h < 7; h++)
					Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + (Vector2.UnitY * 20), Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 6f, ModContent.ProjectileType<Rune>(), Projectile.damage, 0, Projectile.owner);
		}
	}
}
