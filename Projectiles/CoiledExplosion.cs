using Microsoft.Xna.Framework;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class CoiledExplosion : ModProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 80;
			Projectile.timeLeft = 10;
			Projectile.alpha = 255;
			Projectile.maxPenetrate = 1;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			ProjectileExtras.Explode(Projectile.whoAmI, 180, 180,
				delegate {
					for (int i = 0; i < 40; i++)
					{
						int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, -2f, 0, default, Main.rand.NextFloat(.5f, 1.25f));
						Main.dust[num].noGravity = true;
						Dust dust = Main.dust[num];
						dust.position.X += (Main.rand.Next(-50, 51) / 20) - 1.5f;
						dust.position.Y += (Main.rand.Next(-50, 51) / 20) - 1.5f;

						if (Main.dust[num].position != Projectile.Center)
							Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * Main.rand.NextFloat(3f, 6f);
					}
				});
			for (int i = 0; i < 6; i++)
			{
				Gore gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), new Vector2(Projectile.Center.X, Projectile.Center.Y) - new Vector2(24), default, Main.rand.Next(61, 64), Main.rand.NextFloat(1f, 2f));
				gore.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(.5f, 1.8f);
				gore.alpha = 170;

				var unit = Projectile.DirectionTo(Main.player[Projectile.owner].Center);
				ParticleHandler.SpawnParticle(new GlowParticle(Projectile.Center, unit.RotatedByRandom(1f) * Main.rand.NextFloat(2f), Color.Cyan, Main.rand.NextFloat(.25f, 1f), Main.rand.Next(10, 20)));
			}
			ParticleHandler.SpawnParticle(new StarParticle(Projectile.Center, Projectile.DirectionFrom(Main.player[Projectile.owner].Center), Color.Cyan, 1.75f, 10, .5f) { Rotation = Main.rand.NextFloat(MathHelper.Pi) });
		}

		public override void AI() => Projectile.Kill();
	}
}
