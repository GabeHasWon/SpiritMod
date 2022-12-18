using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet.Blaster
{
	public class EnergyBurst : SubtypeProj, IDrawAdditive
	{
		private Color trailColor = Color.White;
		private int[] dustType = new int[2];
		private int debuffType = -1;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Energy Burst");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.alpha = 255;
			Projectile.timeLeft = 80;
			Projectile.height = 8;
			Projectile.width = 8;
			Projectile.extraUpdates = 1;
			AIType = ProjectileID.Bullet;
		}

		public override bool PreAI()
		{
			switch (Subtype)
			{
				case 1:
					trailColor = Color.LimeGreen;
					dustType = new int[] { DustID.FartInAJar, DustID.GreenTorch };
					debuffType = BuffID.Poisoned;
					break;
				case 2:
					trailColor = Color.LightBlue;
					dustType = new int[] { DustID.FrostHydra, DustID.IceTorch };
					debuffType = BuffID.Frostburn;
					break;
				case 3:
					trailColor = Color.Magenta;
					dustType = new int[] { DustID.Pixie, DustID.PinkTorch };
					break;
				default:
					trailColor = Color.Orange;
					dustType = new int[] { DustID.SolarFlare, DustID.Torch };
					debuffType = BuffID.OnFire;
					break;
			}
			return true;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

			if (Projectile.alpha > 0)
				Projectile.alpha -= 255 / 20;
			if (Projectile.alpha < 0)
				Projectile.alpha = 0;
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
			if (Subtype == 3)
			{
				for (int i = 0; i < 10; i++)
				{
					Vector2 velocity = (Projectile.velocity * Main.rand.NextFloat(0.6f, 1.0f)).RotatedByRandom(1.5f);
					Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType[Main.rand.Next(dustType.Length)],
						velocity, 0, default, Main.rand.NextFloat(0.8f, 1.0f));
					dust.noGravity = true;
				}
			}
			else
			{
				for (int i = 0; i < 12; i++)
				{
					Vector2 velocity = (new Vector2(Projectile.velocity.X, 0) * Main.rand.NextFloat(0.3f, 0.5f)).RotatedByRandom(MathHelper.TwoPi);
					if (timeLeft <= 0)
						velocity = (Projectile.velocity * Main.rand.NextFloat(0.6f, 1.0f)).RotatedByRandom(0.11f);
					Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, dustType[Main.rand.Next(dustType.Length)],
						velocity, 0, default, Main.rand.NextFloat(1.0f, 1.2f));
					dust.noGravity = true;
					if (dust.type == DustID.PinkTorch)
						dust.fadeIn = 1.1f;
				}
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (debuffType != -1)
				target.AddBuff(debuffType, 300);

			if (Subtype == 3 && !Main.dedServ)
				ParticleHandler.SpawnParticle(new PlasmaBurst(Projectile.Center, 1f, Main.rand.NextFloat(MathHelper.Pi)));
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			float scale = Projectile.scale;
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = GetDrawFrame(texture);

			Color color = new Color(255, 255, 200) * 0.75f;

			spriteBatch.Draw(texture, Projectile.Center - screenPos, frame, color, Projectile.rotation, frame.Size() / 2, scale * 1.5f, default, default);
			spriteBatch.Draw(texture, Projectile.Center - screenPos, frame, color, Projectile.rotation, frame.Size() / 2, scale * 1.33f, default, default);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle frame = GetDrawFrame(texture);

			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
			{
				float opacityMod = (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
				Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition;
				Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(trailColor) * opacityMod,
					Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White), 
				Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
