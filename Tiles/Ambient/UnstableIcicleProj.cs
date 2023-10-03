using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Ambient
{
	public class UnstableIcicleProj : ModProjectile
	{
		private int Variant { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
		private int Counter { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

		private readonly int warnTime = 12;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Unstable Icicle");
			Main.projFrames[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 5;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(16);
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
		}

		public override void AI()
		{
			Projectile.height = (16 * (Variant + 1)) - 6;
			Projectile.frame = Variant;
			Projectile.tileCollide = Counter > (warnTime + 10); //Allow tile collision after being alive for 10 ticks (after beginning to fall)

			if (++Counter > warnTime)
			{
				const float maxSpeed = 15f;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.UnitY * maxSpeed, .08f);
				Projectile.rotation = 0;
			}
			else
			{
				Projectile.rotation = (float)(Math.Sin(Counter / 2) / 8f);
				Projectile.timeLeft++;

				Dust.NewDustDirect(Projectile.position, Projectile.width, 0, DustID.Snow, 0, 1, 80, default, Main.rand.NextFloat(.7f, 1.1f));
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 10 * (Variant + 1); i++)
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, 0, 0, 100, default, Main.rand.NextFloat(.5f, 1f));

			SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Variant, 0, -2);
			Vector2 drawOffset = new Vector2(Projectile.width / 2, 0);
			Vector2 origin = new Vector2(frame.Width / 2, 0);

			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
			{
				float opacityMod = (ProjectileID.Sets.TrailCacheLength[Type] - i) / (float)ProjectileID.Sets.TrailCacheLength[Type] * .5f;
				Vector2 drawPosition = Projectile.oldPos[i] + drawOffset - Main.screenPosition;

				Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor) * opacityMod, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			}

			Main.EntitySpriteDraw(texture, Projectile.position + drawOffset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
