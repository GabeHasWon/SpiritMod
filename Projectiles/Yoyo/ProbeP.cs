using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Yoyo
{
	public class ProbeP : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("The Probe");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(549);
			Projectile.extraUpdates = 1;
			AIType = 549;
		}

		public override void PostAI() => Projectile.rotation -= 10f;

		public override void AI()
		{
			if (++Projectile.frameCounter < 40)
				return;

			Projectile.frameCounter = 0;
			float num = 8000f;
			int num2 = -1;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				float num3 = Vector2.Distance(Projectile.Center, Main.npc[i].Center);

				if (num3 < num && num3 < 640f && Main.npc[i].CanBeChasedBy(Projectile, false))
				{
					num2 = i;
					num = num3;
				}
			}
			if (num2 != -1)
			{
				bool flag = Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num2].position, Main.npc[num2].width, Main.npc[num2].height);
				if (flag)
				{
					Vector2 velocity = Projectile.DirectionTo(Main.npc[num2].Center) * 8;

					int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + (velocity * 2), velocity, ProjectileID.DeathLaser, (int)(Projectile.damage * 0.6f), Projectile.knockBack / 2f, Projectile.owner, 0f, 0f);
					Main.projectile[p].friendly = true;
					Main.projectile[p].hostile = false;
				}
			}
		}
	}
}
