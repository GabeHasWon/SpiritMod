using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Shockhopper
{
	public class HopperLaser : ModProjectile
	{
		private const int timeLeftMax = 24;

		private Vector2 DirUnit => Vector2.Normalize(Projectile.velocity);
		private int ShotLength = 500;

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults()
			=> DisplayName.SetDefault("Deepspace Hopper");

		public override void SetDefaults()
		{
			Projectile.hostile = true;
			Projectile.width = Projectile.height = 40;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = 8;
			Projectile.timeLeft = timeLeftMax;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
		}

		public override void OnSpawn(IEntitySource source)
		{
			CheckCollision();

			for (int i = 0; i < (ShotLength / 10); i++)
			{
				Vector2 dustPos = Projectile.Center + new Vector2(Main.rand.NextFloat(ShotLength), Main.rand.NextFloat(-(Projectile.height / 2), Projectile.height / 2)).RotatedBy(Projectile.velocity.ToRotation());
				
				Dust.NewDustPerfect(dustPos, DustID.Phantasmal, Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f), 0, Color.White, Main.rand.NextFloat(0.2f, 0.5f)).noGravity = true;
			}

			for (int i = 0; i < 2; i++)
			{
				ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center + (DirUnit * 10), Color.LightBlue, 80 - (i * 30), 20 - (i * 5), PulseCircle.MovementType.OutwardsSquareRooted)
				{
					Angle = Projectile.velocity.ToRotation(),
					ZRotation = 0.6f,
					RingColor = Color.LightBlue,
					Velocity = DirUnit * -(0.5f + i)
				});
			}
		}

		public override bool PreAI()
		{
			Projectile.position -= Projectile.velocity;
			return false;
		}

		private void CheckCollision()
		{
			float[] samples = new float[3];
			Collision.LaserScan(Projectile.Center, DirUnit, 1, ShotLength, samples); //Test tile collision

			ShotLength = 0;
			foreach (float sample in samples)
				ShotLength += (int)(sample / samples.Length);

			foreach (Player player in Main.player) //Test player collision
			{
				float collisionPoint = 0;
				Rectangle hitbox = player.Hitbox;

				if (Collision.CheckAABBvLineCollision(hitbox.TopLeft(), hitbox.Size(), Projectile.Center, Projectile.Center + (DirUnit * ShotLength), Projectile.height, ref collisionPoint))
				{
					ShotLength = (int)(collisionPoint + 1);
					break;
				}
			}

			for (int i = 0; i < 18; i++) //Do impact dusts
			{
				Vector2 velocity = (i < 12) ? -(Projectile.velocity * Main.rand.NextFloat(0.1f, 0.25f)).RotatedByRandom(0.8f) : (Projectile.velocity * Main.rand.NextFloat(0.1f, 0.25f)).RotatedByRandom(0.2f);

				Dust.NewDustPerfect(Projectile.Center + (DirUnit * ShotLength), DustID.Phantasmal, velocity, 0, Color.White, Main.rand.NextFloat(1.0f, 1.5f)).noGravity = true;
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float collisionPoint = 0;

			return (Projectile.timeLeft == timeLeftMax) && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (DirUnit * ShotLength), Projectile.height, ref collisionPoint);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			float quoteant = 1f - (float)((float)Projectile.timeLeft / timeLeftMax);
			float initScaleY = Projectile.height;

			for (int i = 0; i < 3; i++)
			{
				Texture2D texture = (i > 1) ? Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_1").Value : Mod.Assets.Request<Texture2D>("Textures/GlowTrail").Value;
				Vector2 scale = new Vector2(ShotLength, MathHelper.Lerp(initScaleY, 5, quoteant)) / texture.Size();

				Color color = i switch
				{
					0 => Color.DarkCyan * .5f,
					1 => new Color(120, 190, 255) * .8f,
					_ => Color.White * .5f
				};
				color = (color with { A = 0 });
				scale.Y -= 0.03f * i;

				Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.velocity.ToRotation(), new Vector2(0, texture.Height / 2), scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}