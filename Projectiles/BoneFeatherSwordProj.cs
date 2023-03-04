using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class BoneFeatherSwordProj : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private int Distance
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}
		private Vector2 originPos;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bone Feather");
			Main.projFrames[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 2;
			Projectile.extraUpdates = 1;
			Projectile.tileCollide = false;
			Projectile.aiStyle = -1;
		}

		public override void OnSpawn(IEntitySource source)
		{
			originPos = Main.player[Projectile.owner].Center;
			Projectile.frame = Main.rand.Next(Main.projFrames[Type]);
			Projectile.scale = Main.rand.NextFloat(0.8f, 1.2f);

			Projectile.netUpdate = true;
		}

		public override void AI()
		{
			Counter += (int)Projectile.velocity.Length();

			Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
			float rotation = MathHelper.ToRadians(Counter) * Projectile.direction;

			Vector2 posOff = (Vector2.UnitY * -Distance).RotatedBy(rotation);
			posOff.Y *= Projectile.scale;
			Projectile.Center = originPos + posOff - Projectile.velocity;
			Projectile.rotation = rotation + 1.57f + ((Projectile.direction < 0) ? MathHelper.Pi : 0);

			if (Main.rand.NextBool(4))
			{
				Color color = Color.Lerp(Color.White, Color.Yellow, Main.rand.NextFloat(1.0f));
				Dust dust = Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(8.0f)), DustID.Phantasmal, Projectile.velocity / 5, 80, color, Main.rand.NextFloat(0.5f, 1.0f));
				dust.noGravity = true;
			}

			//Fade out
			int fadeTime = 10;
			if (Counter >= (180 - fadeTime))
			{
				Projectile.alpha += 255 / fadeTime;

				if (Projectile.alpha >= 255 || (Counter > 180))
					Projectile.Kill();
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => Projectile.alpha = 80;

		public override bool? CanDamage() => Projectile.alpha <= 0;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			lightColor = Color.White;
			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, (texture.Height / Main.projFrames[Type]) -2);
			Vector2 drawOrigin = frame.Size() / 2;

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}