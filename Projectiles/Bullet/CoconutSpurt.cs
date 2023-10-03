using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet
{
	public class CoconutSpurt : ModProjectile
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Coconut Spurt");

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
			Projectile.Size = new Vector2(14);
			Projectile.penetrate = 1;
			Projectile.extraUpdates = 1;
			Projectile.scale = Main.rand.NextFloat(1, 1.5f);
		}

		public override void OnKill(int timeLeft)
		{
			Vector2 goreVel = Projectile.velocity;
			goreVel.Y *= -0.2f;

			for (int i = 0; i < 5; i++)
			{
				if (i < 2)
				{
					goreVel.X = (i > 0) ? -2 : 2;
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, goreVel, Mod.Find<ModGore>("CoconutSpurtGore").Type, Projectile.scale);
				}

				Vector2 dustVel = Projectile.velocity * -.3f;
				Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BubbleBurst_White, dustVel.X, dustVel.Y, 80, default, Main.rand.NextFloat(1.0f, 2.0f));
			}

			SoundEngine.PlaySound(SoundID.NPCHit18, Projectile.Center);
		}
	}
}
