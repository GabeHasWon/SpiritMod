using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace SpiritMod.Projectiles.Magic
{
	public class Spit : ModProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Diseased Spit");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.alpha = 255;
			Projectile.timeLeft = 60;
		}

		int counter;

		public override bool PreAI()
		{
			const int Repeats = 6;

			for (int k = 0; k < Repeats; k++)
			{
				int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.ScourgeOfTheCorruptor, 0.0f, 0.0f, 0, new Color(), 1f);
				Main.dust[dust].position = Projectile.Center - Projectile.velocity / Repeats * k;
				Main.dust[dust].scale = .75f;
				Main.dust[dust].velocity *= 0f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = false;
			}

			if (++counter >= 1440)
				counter = -1440;
			
			for (int i = 0; i < 20; i++)
			{
				var pos = Projectile.Center + new Vector2(0, (float)Math.Cos(counter / 4.2f) * 9.2f).RotatedBy(Projectile.rotation);
				int num2121 = Dust.NewDust(pos, 6, 6, DustID.ScourgeOfTheCorruptor, 0f, 0f, 0, default, 1f);
				Main.dust[num2121].velocity *= 0f;
				Main.dust[num2121].scale *= .75f;
				Main.dust[num2121].noGravity = true;

			}

			return true;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0)
			{
				int num3 = Main.rand.Next(0, 360);
				for (int j = 0; j < 1; j++)
				{
					float num4 = MathHelper.ToRadians(270 / 1 * j + num3);
					Vector2 vel = Vector2.Normalize(Projectile.velocity.RotatedBy(num4)) * 4.5f;
					var src = Projectile.GetSource_OnHit(target);
					int p = Projectile.NewProjectile(src, Projectile.Center.X, Projectile.Center.Y, vel.X, vel.Y, ProjectileID.TinyEater, Projectile.damage / 5 * 4, 0f, Projectile.owner);
					Main.projectile[p].hostile = false;
					Main.projectile[p].friendly = true;
					Main.projectile[p].DamageType = DamageClass.Magic;
				}
			}
		}
	}
}