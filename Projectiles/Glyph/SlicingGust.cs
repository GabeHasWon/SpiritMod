using Microsoft.Xna.Framework;
using SpiritMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Glyph
{
	public class SlicingGust : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slicing Gust");
			ProjectileID.Sets.TrailCacheLength[Type] = 5;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 220;
			Projectile.Size = new Vector2(14);
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

			if (Main.rand.NextDouble() < 0.5)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position - new Vector2(4, 4), Projectile.width + 8, Projectile.height + 8, ModContent.DustType<Wind>());
				dust.velocity = Projectile.velocity * 0.2f;
				dust.customData = new WindAnchor(Projectile.Center, Projectile.velocity, dust.position);
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.velocity.Y -= knockback * target.knockBackResist;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 6; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Wind>());
				dust.customData = new WindAnchor(Projectile.Center, Projectile.velocity, dust.position);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.QuickDrawTrail(drawColor: Color.White with { A = 0 }, rotation: Projectile.rotation);
			return false;
		}
	}
}
