using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet;

public class GiantBlood : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = 1;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.penetrate = 1;
		Projectile.alpha = 255;
		Projectile.timeLeft = 240;
		AIType = ProjectileID.Bullet;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (target.lifeMax <= 5 || target.dontTakeDamage || target.friendly || target.immortal)
			return;

		Main.player[Projectile.owner].Heal((int)(damageDone * 0.8f));
	}

	public override bool PreAI()
	{
		int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
		Main.dust[dust].scale = 2f;
		Main.dust[dust].noGravity = true;
		Main.dust[dust].noLight = true;
		return false;
	}
}
