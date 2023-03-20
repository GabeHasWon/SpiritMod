using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public class PhaseBlast : SubtypeProj, IDrawAdditive
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phase Blast");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.alpha = 255;
			Projectile.timeLeft = 30;
			Projectile.height = 20;
			Projectile.width = 20;
			Projectile.penetrate = -1;
			Projectile.extraUpdates = 1;
			Projectile.tileCollide = false;
			AIType = ProjectileID.Bullet;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			Projectile.velocity *= 0.96f;

			if (Projectile.alpha > 0)
				Projectile.alpha -= 255 / 20;
			if (Projectile.alpha < 0)
				Projectile.alpha = 0;

			for (int i = 0; i < 2; i++)
			{
				int[] dustType = Dusts;

				Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType[Main.rand.Next(dustType.Length)], Projectile.velocity * .5f, 80);
				dust.noGravity = true;
			}
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
			for (int i = 0; i < 12; i++)
			{
				Vector2 velocity = (new Vector2(Projectile.velocity.X, 0) * Main.rand.NextFloat(0.3f, 0.5f)).RotatedByRandom(MathHelper.TwoPi);
				if (timeLeft <= 0)
					velocity = (Projectile.velocity * Main.rand.NextFloat(0.6f, 1.0f)).RotatedByRandom(0.11f);

				int[] dustType = Dusts;
				Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, dustType[Main.rand.Next(2)],
					velocity, 0, default, Main.rand.NextFloat(1.0f, 1.2f));
				dust.noGravity = true;

				if (dust.type == DustID.PinkTorch)
					dust.fadeIn = 1.1f;
			}
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Color color = GetColor(Subtype) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				float scale = Projectile.scale;
				Texture2D texture = TextureAssets.Projectile[Type].Value;

				spriteBatch.Draw(texture, Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2, scale, default, default);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Color drawColor = GetColor(Subtype);

			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
			{
				float opacityMod = (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
				Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition;
				Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(drawColor) * opacityMod,
					Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(drawColor), 
				Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
