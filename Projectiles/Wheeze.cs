using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class Wheeze : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Wheeze Gas");
			Main.projFrames[Projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.penetrate = 5;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(20, 150);

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			if (info.PvP)
				target.AddBuff(20, 150);
		}
		public override bool PreAI() 
		{
			Projectile.alpha += 2;
			if (Projectile.alpha >= 255)
				Projectile.Kill();
			return true;
		}

		public override void AI()
		{
			Projectile.velocity *= 0.92f;
			Projectile.spriteDirection = Projectile.direction;
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 6) {
				Projectile.frame++;
				Projectile.frameCounter = 0;
				if (Projectile.frame >= 8)
					Projectile.frame = 0;
			}
		}
	}
}
