using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic
{
	public class IchorClotProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ichor Clot");
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 25;
			Projectile.timeLeft = 3000;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.height = 25;
			Projectile.aiStyle = -1;
			Projectile.alpha = 255;
		}

		public override void AI()
		{
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, Main.rand.Next(-3, 3), Main.rand.Next(-3, 3));
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = 2f;
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 40) {
				Projectile.frameCounter = 0;
				float num = 8000f;
				int num2 = -1;
				for (int i = 0; i < 200; i++) {
					float num3 = Vector2.Distance(Projectile.Center, Main.npc[i].Center);
					if (num3 < num && num3 < 640f && Main.npc[i].CanBeChasedBy(Projectile, false)) {
						num2 = i;
						num = num3;
					}
				}

				if (num2 != -1) {
					bool flag = Collision.CanHit(Projectile.position, Projectile.width,
						Projectile.height, Main.npc[num2].position,
						Main.npc[num2].width, Main.npc[num2].height);
					if (flag) {
						Vector2 value = Main.npc[num2].Center - Projectile.Center;
						float num4 = 9f;
						float num5 = (float)Math.Sqrt((double)(value.X * value.X + value.Y * value.Y));
						if (num5 > num4) {
							num5 = num4 / num5;
						}
						value *= num5;
						int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y,
							value.X, value.Y, ProjectileID.GoldenShowerFriendly,
							Projectile.damage, Projectile.knockBack * .5f, Projectile.owner);
						Main.projectile[p].friendly = true;
						Main.projectile[p].hostile = false;
					}
				}
			}
		}

	}
}
