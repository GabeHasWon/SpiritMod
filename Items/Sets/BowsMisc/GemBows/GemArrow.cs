using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BowsMisc.GemBows
{
	public abstract class GemArrow : ModProjectile
	{
		protected int dustType;
		protected Color glowColor;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.arrow = true;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.alpha = 80;

			SafeSetDefaults();
		}
		protected virtual void SafeSetDefaults() { }

		public override Color? GetAlpha(Color lightColor) => Color.White * (1f - (float)Projectile.alpha / 255f);
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			float pulse = (float)(Math.Cos(Main.GlobalTimeWrappedHourly % 2.40000009536743 / 2.40000009536743 * MathHelper.TwoPi) / 4.0f + 0.5f);
			Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
			{
				float opacityMod = (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type] * (.5f - (pulse * .25f));
				Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
				Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor) * opacityMod,
					Projectile.oldRot[i], texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}
			Color color = new Color(sbyte.MaxValue - Projectile.alpha, sbyte.MaxValue - Projectile.alpha, sbyte.MaxValue - Projectile.alpha, 0).MultiplyRGBA(Color.White);

			Main.EntitySpriteDraw(texture, drawPos, null, Projectile.GetAlpha(color), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			for (int i = 0; i < 3; i++)
				Main.EntitySpriteDraw(texture, drawPos, null, color * (1f + i - pulse), Projectile.rotation, texture.Size() / 2, Projectile.scale + (pulse * (.7f / (i + 1))), SpriteEffects.None, 0);

			Lighting.AddLight(Projectile.Center, glowColor.ToVector3() / 2f);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			for (int index = 0; index < 5; ++index)
			{
				int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, glowColor, 1f);
				Main.dust[i].noGravity = true;
			}
			SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.4f });
		}
	}
}