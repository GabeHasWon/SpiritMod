using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.DoT;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic
{
	class BismiteShard : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bismite Energy");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 220;
			Projectile.height = 14;
			Projectile.width = 10;
			Projectile.DamageType = DamageClass.Magic;
			AIType = ProjectileID.Bullet;
			Projectile.extraUpdates = 1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
			return false;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			Projectile.alpha += 2;
			Projectile.velocity *= 0.98f;

			Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GreenFairy, Projectile.velocity, Projectile.alpha);
			dust.noGravity = true;
		}

		public override Color? GetAlpha(Color lightColor) => new Color(155 - (int)(Projectile.alpha / 3f * 2), 204 - (int)(Projectile.alpha / 3f * 2), 92 - (int)(Projectile.alpha / 3f * 2), 255 - Projectile.alpha);

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<FesteringWounds>(), 180);
		}
	}
}
