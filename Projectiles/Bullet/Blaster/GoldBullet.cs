using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet.Blaster
{
	public class GoldBullet : ModProjectile, ITrailProjectile
	{
		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new GradientTrail(Color.White, Color.Yellow), new RoundCap(), new DefaultTrailPosition(), 10f, 704f, new DefaultShader());
			tManager.CreateTrail(Projectile, new GradientTrail(Color.Orange, Color.Red * .5f), new RoundCap(), new DefaultTrailPosition(), 10f, 704f, new DefaultShader());
			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.White), new TriangleCap(), new DefaultTrailPosition(), 2f, 700f, new DefaultShader());
		}

		public override void SetStaticDefaults() => DisplayName.SetDefault("Gold Bullet");
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 80;
			Projectile.height = 6;
			Projectile.width = 6;
			AIType = ProjectileID.Bullet;
			Projectile.extraUpdates = 1;
		}

		private readonly int timerMax = 20;
		private int timer;
		public override void AI()
		{
			Vector2 position = Projectile.Center + new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * 10f, Main.rand.NextFloat(-1.0f, 1.0f) * 10f);
			for (int i = 0; i < 2; i++)
			{
				int num = Dust.NewDust(position, 0, 0, DustID.IchorTorch, 0, 0, 100, default, 0.8f);
				Main.dust[num].velocity = Vector2.Zero;
				Main.dust[num].noGravity = true;
			}
			if (timer == 0 && !Main.dedServ)
				ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center + (Projectile.velocity * 3), Color.LightBlue, 20, 10)
					{ Angle = Projectile.velocity.ToRotation(), ZRotation = 0.5f });

			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			timer = ++timer % timerMax;
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
			for (int i = 0; i < 14; i++)
			{
				Vector2 velocity = (Projectile.velocity * Main.rand.NextFloat(0.6f, 1.0f)).RotatedByRandom(1f);
				Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, Main.rand.NextBool(2) ? DustID.IchorTorch : DustID.GemTopaz,
					velocity, 0, default, Main.rand.NextFloat(0.8f, 1.5f));
				dust.noGravity = true;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), 
				Projectile.rotation, TextureAssets.Projectile[Projectile.type].Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
