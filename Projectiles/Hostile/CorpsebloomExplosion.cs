using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Hostile
{
	public class CorpsebloomExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Corpsebloom Explosion");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.hide = true;
			Projectile.alpha = 255;
			Projectile.timeLeft = 6;
		}

		public override void AI()
		{
			for (int i = 0; i < Main.maxPlayers; ++i)
			{
				if (Vector2.DistanceSquared(Projectile.Center, Main.player[i].Center) < 80 * 80)
					Main.player[i].AddBuff(BuffID.Poisoned, 480);
			}

			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				if (!Main.npc[i].dontTakeDamage && Vector2.DistanceSquared(Projectile.Center, Main.npc[i].Center) < 80 * 80)
					Main.npc[i].AddBuff(BuffID.Poisoned, 480);
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 40; i++)
			{
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green, 0f, -2f, 0, Color.Purple, .8f);
				Main.dust[num].noLight = true;
				Dust dust = Main.dust[num];
				dust.position.X = dust.position.X + ((float)(Main.rand.Next(-40, 41) / 20) - 1.5f);
				Dust expr_92_cp_0 = Main.dust[num];
				expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + ((float)(Main.rand.Next(-40, 41) / 20) - 1.5f);
				if (Main.dust[num].position != Projectile.Center)
				{
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 3f;
				}
			}
		}
	}
}