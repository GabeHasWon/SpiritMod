using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Mechanics.Trails;

namespace SpiritMod.Projectiles.Bullet
{
	public class TerraBullet1 : ModProjectile, ITrailProjectile
	{
		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new GradientTrail(Color.Green with { A = 0 }, Color.Blue with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 10f, 500f, new DefaultShader());
			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.White), new RoundCap(), new DefaultTrailPosition(), 5f, 500f, new DefaultShader());
		}

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Energy Bolt");

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 4;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.tileCollide = true;
			Projectile.penetrate = 2;
			Projectile.timeLeft = 300;
			Projectile.extraUpdates = 1;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.BlueCrystalShard, null, Projectile.alpha);
			dust.velocity = (Projectile.velocity * Main.rand.NextFloat(.1f, .3f)).RotatedByRandom(.15f);
			dust.noGravity = true;

			bool flag25 = false;
			int jim = 1;
			for (int index1 = 0; index1 < 200; index1++) {
				if (Main.npc[index1].CanBeChasedBy(Projectile, false)
					&& Projectile.Distance(Main.npc[index1].Center) < 500
					&& Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1)) {
					flag25 = true;
					jim = index1;
				}
			}

			if (flag25) {
				float num1 = 6f;
				Vector2 vector2 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
				float num2 = Main.npc[jim].Center.X - vector2.X;
				float num3 = Main.npc[jim].Center.Y - vector2.Y;
				float num4 = (float)Math.Sqrt((double)num2 * num2 + num3 * num3);
				float num5 = num1 / num4;
				float num6 = num2 * num5;
				float num7 = num3 * num5;
				int num8 = 10;
				Projectile.velocity.X = (Projectile.velocity.X * (num8 - 1) + num6) / num8;
				Projectile.velocity.Y = (Projectile.velocity.Y * (num8 - 1) + num7) / num8;
			}
		}
	}
}
