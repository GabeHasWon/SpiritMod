using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet
{
	public class TerraBomb : ModProjectile, ITrailProjectile
	{
		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new GradientTrail(Color.Yellow with { A = 0 }, Color.Green with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 25f, 100f, new DefaultShader());
			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.White with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 10f, 100f, new DefaultShader());
		}

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Elemental Bomb");

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.SpikyBall);
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 150;
			Projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
			const int timeToDetonate = 40;

			if (Projectile.alpha < 100)
				Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade).noGravity = true;
			if (Projectile.timeLeft < timeToDetonate)
			{
				if (Projectile.timeLeft % 10 == 0)
					ParticleHandler.SpawnParticle(new PulseCircle(Projectile, (Color.Green with { A = 0 }) * (1f - Projectile.Opacity), 80, 8, PulseCircle.MovementType.Inwards, Projectile.Center));
				if (Projectile.timeLeft % 2 == 0)
				{
					Vector2 linePos = Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(25f, 80f));
					ParticleHandler.SpawnParticle(new ImpactLine(linePos, linePos.DirectionTo(Projectile.Center) * 2.5f, Color.Yellow * (1f - Projectile.Opacity), new Vector2(.5f, 1f), 15, Projectile));
				}

				Projectile.alpha = Math.Min(255, Projectile.alpha + (255 / timeToDetonate));
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(0, -1), .1f);
				return false;
			}
			return true;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			ProjectileExtras.Explode(Projectile.whoAmI, 120, 120, delegate
			{
				for (int i = 0; i < 40; i++)
				{
					Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, Vector2.Zero, 0, default, Main.rand.NextFloatDirection() + 1.5f);
					dust.noGravity = true;
					dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloatDirection() * 6f;

					if (i < 3)
						Gore.NewGoreDirect(Projectile.GetSource_Death(), new Vector2(Projectile.position.X + (Projectile.width / 2) - 24f, Projectile.position.Y + (Projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f).velocity *= Main.rand.NextFloat();
				}
			});
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			int debuffType = Main.rand.Next(3) switch
			{
				1 => BuffID.OnFire3,
				2 => BuffID.Frostburn2,
				_ => BuffID.CursedInferno
			};
			target.AddBuff(debuffType, 300, true);
		}
	}
}

