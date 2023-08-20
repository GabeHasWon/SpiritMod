using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public class PiercingBeam : SubtypeProj
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int counterMax = 14;

		private int shotLength = 1200;
		private Vector2 origin;

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Energy Beam");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (Counter == 0)
			{
				origin = Projectile.Center;

				for (int i = 0; i < 12; i++) //Do impact dusts
				{
					Dust dust = Dust.NewDustPerfect(origin + (Vector2.Normalize(Projectile.velocity) * shotLength), Dusts[Main.rand.Next(2)], Vector2.Zero, 0, Color.White, Main.rand.NextFloat(1.0f, 1.5f));
					dust.velocity = -(Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f)).RotatedByRandom(0.8f);
					dust.noGravity = true;
				}
			}
			else Projectile.Center = player.Center;

			if (Counter < counterMax)
				Counter++;
			if (player.itemAnimation < 2)
				Projectile.Kill();
		}

		public override bool? CanDamage() => Counter == 1;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 dirUnit = Vector2.Normalize(Projectile.velocity);

			//Test tile collision
			float[] samples = new float[3];
			Collision.LaserScan(origin, dirUnit, 1, shotLength, samples);

			shotLength = 0;
			foreach (float sample in samples)
				shotLength += (int)(sample / samples.Length);

			float collisionPoint = shotLength;
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), origin, origin + (dirUnit * shotLength), 30, ref collisionPoint))
				return true;

			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (Counter < 1)
				return false;

			bool shrink = Counter > (counterMax / 2);

			float quoteant = (float)Counter / counterMax;
			float maxScaleY = 80;

			for (int i = 0; i < 3; i++)
			{
				Texture2D texture = (i > 1) ? Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_1").Value : Mod.Assets.Request<Texture2D>("Textures/GlowTrail").Value;
				Vector2 scale = new Vector2(shotLength, MathHelper.Lerp(maxScaleY * 0.75f, shrink ? 0 : maxScaleY, shrink ? quoteant * 0.01f : quoteant)) / texture.Size();

				Color color = i switch
				{
					0 => GetColor(Subtype) * .4f,
					1 => GetColor(Subtype) * .8f,
					_ => Color.White * 1.5f
				};
				color = (color with { A = 0 }) * (float)(1f - quoteant);

				Main.EntitySpriteDraw(texture, origin - Main.screenPosition, null, color, Projectile.velocity.ToRotation(), new Vector2(0, texture.Height / 2), scale, SpriteEffects.None, 0);

				for (int o = 0; o < 2; o++)
				{
					Texture2D bloom = Mod.Assets.Request<Texture2D>("Effects/Masks/CircleGradient").Value;
					Vector2 endPos = origin + ((Vector2.UnitX * shotLength).RotatedBy(Projectile.velocity.ToRotation()) * o);
					Main.spriteBatch.Draw(bloom, endPos - Main.screenPosition, null, color, 0, bloom.Size() / 2, (0.3f - (i * 0.03f)) * (float)(1f - (quoteant * 0.6f)), SpriteEffects.None, 0);
				}
			}

			if (Counter == 1) //Do fancy dusts
			{
				for (int i = 0; i < (shotLength / 10); i++)
				{
					Vector2 dustPos = Projectile.Center + new Vector2(Main.rand.NextFloat(shotLength), Main.rand.NextFloat(-(maxScaleY / 4), maxScaleY / 4)).RotatedBy(Projectile.velocity.ToRotation());
					Dust dust = Dust.NewDustPerfect(dustPos, Dusts[Main.rand.Next(2)], Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.5f, 1.0f));
					dust.velocity = Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f);
					dust.noGravity = true;
				}
			}

			return false;
		}
	}
}