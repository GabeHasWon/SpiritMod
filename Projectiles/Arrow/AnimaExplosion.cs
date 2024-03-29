using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using SpiritMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Arrow
{
	public class AnimaExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Anima");
		}

		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.alpha = 255;
			Projectile.timeLeft = 1;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			ProjectileExtras.Explode(Projectile.whoAmI, 60, 60,
			delegate {
				for (int i = 0; i < 60; i++) {
					int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<NightmareDust>(), 0f, -2f, 0, default, 1.1f);
					Main.dust[num].noGravity = true;
					Main.dust[num].scale = 1.5f;
					Dust dust = Main.dust[num];
					dust.position.X = dust.position.X + ((float)(Main.rand.Next(-30, 31) / 20) - 1.5f);
					Dust expr_92_cp_0 = Main.dust[num];
					expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + ((float)(Main.rand.Next(-30, 31) / 20) - 1.5f);
					if (Main.dust[num].position != Projectile.Center) {
						Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 3f;
					}
				}
			});
			Projectile.active = false;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (target.type != NPCID.TargetDummy) {
				target.AddBuff(ModContent.BuffType<DrainLife>(), 150, true);
			}
		}

	}
}