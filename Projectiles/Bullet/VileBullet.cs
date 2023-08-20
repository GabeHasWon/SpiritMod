using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using SpiritMod.Mechanics.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet
{
	public class VileBullet : ModProjectile, ITrailProjectile
	{
		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new GradientTrail(Color.Yellow with { A = 0 }, Color.Purple with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 10f, 500f, new DefaultShader());
			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.Magenta with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 6f, 500f, new DefaultShader());
		}

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Vile Bullet");
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 240;
			Projectile.alpha = 255;
			AIType = ProjectileID.Bullet;
		}

		public override void AI()
		{
			Projectile.alpha = 255;
			bool flag25 = false;
			int jim = 1;
			for (int index1 = 0; index1 < 200; index1++) {
				if (Main.npc[index1].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1) && Main.npc[index1].HasBuff(ModContent.BuffType<Tracked>())) {
					float num23 = Main.npc[index1].position.X + (float)(Main.npc[index1].width / 2);
					float num24 = Main.npc[index1].position.Y + (float)(Main.npc[index1].height / 2);
					float num25 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num23) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num24);
					if (num25 < 500f) {
						flag25 = true;
						jim = index1;
					}
				}
			}

			int dustType;
			if (flag25)
			{
				float num1 = 12f;
				Vector2 vector2 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
				float num2 = Main.npc[jim].Center.X - vector2.X;
				float num3 = Main.npc[jim].Center.Y - vector2.Y;
				float num4 = (float)Math.Sqrt((double)num2 * (double)num2 + (double)num3 * (double)num3);
				float num5 = num1 / num4;
				float num6 = num2 * num5;
				float num7 = num3 * num5;
				int num8 = 10;

				dustType = 173;

				Projectile.velocity.X = (Projectile.velocity.X * (float)(num8 - 1) + num6) / (float)num8;
				Projectile.velocity.Y = (Projectile.velocity.Y * (float)(num8 - 1) + num7) / (float)num8;
			}
			else dustType = 27;

			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType, null, Projectile.alpha);
				dust.velocity = (Projectile.velocity * Main.rand.NextFloat(.1f, .3f)).RotatedByRandom(.15f);
				dust.noGravity = true;
			}
		}
	}
}
