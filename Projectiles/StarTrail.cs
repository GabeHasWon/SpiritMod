using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;

namespace SpiritMod.Projectiles
{
	public class StarTrail : ModProjectile
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Star Trail");
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.timeLeft = 255;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.alpha = 0;
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
		}

		public override void AI()
		{
			float maxValue = 255 / 2;
			float scalar = (float)(Projectile.timeLeft / maxValue);
			if (scalar > 1)
				scalar -= (scalar - 1) * 2;

			Projectile.velocity *= 0.98f;

			Projectile.spriteDirection = (Projectile.velocity.X < 0) ? -1 : 1;
			Projectile.scale = scalar * .6f;
			Projectile.rotation += 0.04f * Projectile.spriteDirection;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Color color = Projectile.GetAlpha(Color.White);
			color.A = 0;
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, 
				TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => Projectile.numHits++;
		public override bool? CanDamage() => (Projectile.numHits < 1) ? null : false;
	}
}
