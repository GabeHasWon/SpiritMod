using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public class Bauble : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bauble");
			Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 200;
			Projectile.height = 6;
			Projectile.width = 6;
			AIType = ProjectileID.Bullet;
		}

		public override void OnSpawn(IEntitySource source) => Projectile.frame = Main.rand.Next(Main.projFrames[Type]);

		public override void AI()
		{
			Vector2 position = Projectile.Center + new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * 10f, Main.rand.NextFloat(-1.0f, 1.0f) * 10f);
			if (Main.rand.NextBool(10))
			{
				int num = Dust.NewDust(position, 0, 0, DustID.GoldCoin, 0, 0, 100, default, 0.8f);
				Main.dust[num].velocity = Vector2.Zero;
				Main.dust[num].noGravity = true;
			}

			Projectile.velocity.Y += 0.2f;

			Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X < 0) ? -1 : 1;
			Projectile.rotation += 0.1f * Projectile.direction;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			float mult = 0.8f;

			if (Math.Abs(Projectile.velocity.X) > Math.Abs(Projectile.velocity.Y))
				Projectile.velocity.Y = -(oldVelocity.Y * mult);
			else
				Projectile.velocity.X = -(oldVelocity.X * mult);

			if (oldVelocity.Length() > 3f)
				SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.14f, PitchVariance = 0.2f }, Projectile.Center);

			return false;
		}

		public override void Kill(int timeLeft)
		{
			if (timeLeft > 0)
				SoundEngine.PlaySound(SoundID.Shatter, Projectile.position);

			for (int i = 0; i < 14; i++)
			{
				Vector2 velocity = -(Projectile.velocity * Main.rand.NextFloat(0.4f, 0.8f)).RotatedByRandom(1f);
				Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, Main.rand.NextBool(2) ? DustID.Confetti : DustID.GoldCoin,
					velocity, 0, default, Main.rand.NextFloat(0.8f, 1.5f));
				if (dust.type == DustID.GoldCoin)
				{
					dust.noGravity = true;
					dust.velocity = dust.velocity.RotatedByRandom(2f);
				}
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.QuickDrawTrail(Main.spriteBatch, 0.75f, Projectile.rotation, SpriteEffects.None);

			Rectangle frame = new Rectangle(0, TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Type] * Projectile.frame, TextureAssets.Projectile[Projectile.type].Value.Width, (TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Type]) - 2);

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), 
				Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
