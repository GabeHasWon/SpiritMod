using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using SpiritMod.Dusts;
using System;

namespace SpiritMod.Projectiles.Thrown
{
	public class MimeBombProj : ModProjectile
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Mime Bomb");

		public override void SetDefaults()
		{
			Projectile.width = 17;
			Projectile.height = 17;
			Projectile.aiStyle = 2;
			Projectile.timeLeft = 180;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.alpha = 0;
			Projectile.penetrate = -1;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.Kill();

		public override bool PreAI()
		{
			Projectile.velocity.X *= 1.015f;
			return base.PreAI();
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
				Projectile.velocity.X = oldVelocity.X * -0.45f;
			if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
				Projectile.velocity.Y = oldVelocity.Y * -0.45f;

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			ProjectileExtras.Explode(Projectile.whoAmI, 120, 45, delegate
			{
				DustHelper.DrawDustImage(Projectile.position, ModContent.DustType<MarbleDust>(), 0.075f, "SpiritMod/Effects/DustImages/Boom", 1.66f);
			});
		}
	}
}
