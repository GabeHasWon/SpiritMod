using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet
{
	public class TerraBullet : ModProjectile, ITrailProjectile
	{
		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new GradientTrail(Color.Yellow with { A = 0 }, Color.Green with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 10f, 500f, new DefaultShader());
			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.White with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 6f, 500f, new DefaultShader());
		}

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Terra Bullet");

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 4;
			AIType = ProjectileID.Bullet;
			Projectile.penetrate = 2;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
		}

		public override void AI()
		{
			Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, null, 100, default, Main.rand.NextFloat(.5f, 1f));
			dust.noGravity = true;
			dust.velocity = (Projectile.velocity * Main.rand.NextFloat(.2f, .3f)).RotatedByRandom(.15f);
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
