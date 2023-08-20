using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Boss.MoonWizard.Projectiles
{
	public class SineBall : ModProjectile
	{
		float distance = 8;
		readonly int rotationalSpeed = 4;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Focus Ball");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
			Projectile.width = Projectile.height = 32;
		}

        float alphaCounter;
		Vector2 initialSpeed = Vector2.Zero;

		public override void OnSpawn(IEntitySource source) => initialSpeed = Projectile.velocity;

		public override void AI()
		{
            Projectile.velocity *= 1.009f;
            alphaCounter += 0.04f;

            int rightValue = (int)Projectile.ai[1] - 1;
			if (rightValue < (double)Main.projectile.Length && rightValue != -1) 
			{
				Projectile other = Main.projectile[rightValue];
				Vector2 direction9 = other.Center - Projectile.Center;
				int distance = (int)Math.Sqrt((direction9.X * direction9.X) + (direction9.Y * direction9.Y));
				direction9.Normalize();

				if (Projectile.timeLeft % 4 == 0 && distance < 1000 && other.active)
					DustHelper.DrawElectricity(Projectile.Center + (Projectile.velocity * 4), other.Center + (other.velocity * 4), 226, 0.35f, 30, default, 0.12f);
			}

			Projectile.spriteDirection = 1;
			if (Projectile.ai[0] > 0)
				Projectile.spriteDirection = 0;

			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			distance += 0.19f;
			Projectile.ai[0] += rotationalSpeed;
			
			Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
			offset.Normalize();
			offset *= (float)(Math.Cos(Projectile.ai[0] * (Math.PI / 180)) * (distance / 3));
			Projectile.velocity = initialSpeed + offset;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 lineStart = Projectile.Center;
			int rightValue = (int)Projectile.ai[1] - 1;
			float collisionpoint = 0f;

			if (rightValue < (double)Main.projectile.Length && rightValue != -1)
			{
				Projectile other = Main.projectile[rightValue];
				Vector2 lineEnd = other.Center;

				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, lineEnd, Projectile.scale / 2, ref collisionpoint))
					return true;
			}
			return false;
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;

		public override void PostDraw(Color lightColor)
        {
            float sineAdd = (float)Math.Sin(alphaCounter) + 3;
            Main.EntitySpriteDraw(Terraria.GameContent.TextureAssets.Extra[49].Value, (Projectile.Center - Main.screenPosition), null, new Color((int)(7.5f * sineAdd), (int)(16.5f * sineAdd), (int)(18f * sineAdd), 0), 0f, new Vector2(50, 50), 0.25f * (sineAdd + .6f), SpriteEffects.None, 0);
        }

		public override void Kill(int timeLeft) => DustHelper.DrawDustImage(Projectile.Center, 226, 0.15f, "SpiritMod/Effects/DustImages/MoonSigil3", 1f);
	}
}