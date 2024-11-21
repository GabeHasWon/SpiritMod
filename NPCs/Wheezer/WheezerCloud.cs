using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Wheezer
{
	public class WheezerCloud : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gas Cloud");
			Main.projFrames[Projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.light = 0;
			Projectile.timeLeft = 255;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.scale *= .8f;
		}

		public override void AI()
		{
			Projectile.velocity *= .95f;

			Projectile.alpha += 1;

			Projectile.spriteDirection = Projectile.direction;
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 6) {
				Projectile.frame++;
				Projectile.frameCounter = 0;
				if (Projectile.frame >= 8)
					Projectile.frame = 0;
			}
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			if (Main.rand.NextBool(2))
				target.AddBuff(BuffID.Poisoned, 300);
		}
	}
}