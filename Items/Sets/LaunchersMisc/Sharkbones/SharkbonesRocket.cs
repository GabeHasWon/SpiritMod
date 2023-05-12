using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.BaseProj;
using Terraria;
using SpiritMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Prim;
using Terraria.Audio;

namespace SpiritMod.Items.Sets.LaunchersMisc.Sharkbones
{
	public class SharkbonesRocket : BaseRocketProj, ITrailProjectile, IDrawAdditive
	{
		public void DoTrailCreation(TrailManager tM)
		{
			tM.CreateTrail(Projectile, new GradientTrail(Color.Cyan * .5f, Color.LightPink), new RoundCap(), new DefaultTrailPosition(), 22, 450);
			tM.CreateTrail(Projectile, new StandardColorTrail(Color.DarkCyan with { A = 50 }), new NoCap(), new DefaultTrailPosition(), 12, 350);
			tM.CreateTrail(Projectile, new GradientTrail(Color.CornflowerBlue with { A = 50 }, Color.Transparent), new NoCap(), new DefaultTrailPosition(), 60, 500, new ImageShader(Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_1").Value, 0.05f, 1f, 1f));
		}

		public override void SetStaticDefaults() => DisplayName.SetDefault("Super Mega Death Rocket");

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.width = Projectile.height = 24;
			Projectile.timeLeft = 240;
		}

		public override int ExplosionRange => 200;
		private readonly float targetSpeed = 15f;

		public override void AI()
		{
			NPC target = Projectiles.ProjectileExtras.FindNearestNPC(Projectile.Center, 800, false);
			if (target != null)
				Projectile.velocity = Projectile.velocity.Length() * Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * Projectile.velocity.Length(), 0.05f));

			if (Projectile.velocity.Length() < targetSpeed)
				Projectile.velocity *= 1.06f;

			Projectile.rotation = Projectile.velocity.ToRotation();

			if (!Main.dedServ)
			{
				ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(.3f) * Main.rand.NextFloat(),
					Color.White, Color.Cyan, Main.rand.NextFloat(0.35f, 0.8f), 7));

				for (int i = 0; i < 2; i++)
					ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, -Projectile.velocity.RotatedByRandom(MathHelper.Pi / 16) * Main.rand.NextFloat(0.25f), Color.DarkCyan with { A = 50 }, Main.rand.NextFloat(0.2f, 0.3f), 20));
			}
		}

		public override bool PreKill(int timeLeft)
		{
			Projectile.damage /= 2;
			return base.PreKill(timeLeft);
		}

		public override void ExplodeEffect()
		{
			for (int i = 0; i < 40; i++)
				Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2Unit() * Main.rand.NextFloat(6.0f), 100, Color.Lerp(Color.Red, Color.Blue, Main.rand.NextFloat()), .7f);

			ParticleHandler.SpawnParticle(new SharkbonesExplosion(Projectile.Center, 1, Main.rand.NextFloat(-.2f, .2f)));
			SoundEngine.PlaySound(SoundID.Item14 with { PitchVariance = 0.2f }, Projectile.Center);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			SpriteEffects effects = (Projectile.direction == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			float rotation = (effects == SpriteEffects.FlipHorizontally) ? Projectile.rotation + MathHelper.Pi : Projectile.rotation;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), rotation, texture.Size() / 2, Projectile.scale, effects, 0);

			return false;
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			Effect blurEffect = ModContent.Request<Effect>("SpiritMod/Effects/BlurLine").Value;
			float quoteant = 1f - (float)((float)Projectile.velocity.Length() / targetSpeed);

			IPrimitiveShape[] shapes = new IPrimitiveShape[]
			{
				new SquarePrimitive()
				{
					Position = Projectile.Center - (Vector2.Normalize(Projectile.velocity) * 22) - screenPos,
					Height = 400 * quoteant,
					Length = 50 * quoteant,
					Rotation = 1.57f,
					Color = Color.LightPink
				},
				new SquarePrimitive()
				{
					Position = Projectile.Center - (Vector2.Normalize(Projectile.velocity) * 22) - screenPos,
					Height = 300 * quoteant,
					Length = 35 * quoteant,
					Rotation = Projectile.rotation,
					Color = Color.LightPink * .7f
				}
			};
			PrimitiveRenderer.DrawPrimitiveShapeBatched(shapes, blurEffect);
		}
	}
}