using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public class EnergyBeam : SubtypeProj
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int counterMax = 10;

		private int ShotLength
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}
		public const int shotLengthMax = 500;

		private Vector2 origin;
		private int targetWhoAmI = -1;

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Energy Beam");

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
				CheckCollision();
			}
			else Projectile.Center = player.Center;

			if (Counter < counterMax)
				Counter++;
			if (player.itemAnimation < 2)
				Projectile.Kill();
		}

		public override bool? CanDamage() => Counter == 1;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => (targetWhoAmI != -1) && (Main.npc[targetWhoAmI].Hitbox == targetHitbox);

		private void CheckCollision()
		{
			Vector2 dirUnit = Vector2.Normalize(Projectile.velocity);

			float lenience = 8f; //The length from the origin that the projectile won't scan for tile collision

			//Test tile collision
			float[] samples = new float[3];
			Collision.LaserScan(origin + (dirUnit * lenience), dirUnit, 1, ShotLength - lenience, samples);

			ShotLength = 0;
			foreach (float sample in samples)
				ShotLength += (int)(sample / samples.Length);

			bool doDusts = false;
			//Test NPC collision
			foreach (NPC npc in Main.npc)
			{
				float collisionPoint = ShotLength;
				if (Collision.CheckAABBvLineCollision(npc.Hitbox.TopLeft(), npc.Hitbox.Size(), origin, origin + (dirUnit * ShotLength), 1, ref collisionPoint) && npc.active && !npc.friendly && npc.CanDamage())
				{
					if (collisionPoint < ShotLength)
					{
						ShotLength = (int)collisionPoint;
						targetWhoAmI = npc.whoAmI;
						doDusts = true; //Get the first NPC to the player regardless of their position in the array
					}
				}
			}

			if ((ShotLength + lenience) < shotLengthMax) //The projectile has collided with a tile
			{
				if (bouncy)
				{
					Vector2 bounceVel = -Projectile.velocity.RotatedByRandom(1.57f);

					Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), origin + (dirUnit * ShotLength), bounceVel, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, shotLengthMax - ShotLength);

					if (proj.ModProjectile is SubtypeProj)
						(proj.ModProjectile as SubtypeProj).Subtype = Subtype;
				}
				doDusts = true;
			}

			if (!doDusts)
				return;
			for (int i = 0; i < 12; i++) //Do impact dusts
			{
				Dust dust = Dust.NewDustPerfect(origin + (dirUnit * ShotLength), Dusts[Main.rand.Next(2)], Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.9f, 1.2f));
				dust.velocity = -(Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f)).RotatedByRandom(0.8f);
				dust.noGravity = true;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (Counter < 1)
				return false;

			float quoteant = (float)Counter / counterMax;
			float initScaleY = 30;

			for (int i = 0; i < 3; i++)
			{
				Texture2D texture = (i > 1) ? Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_1").Value : Mod.Assets.Request<Texture2D>("Textures/GlowTrail").Value;
				Vector2 scale = new Vector2(ShotLength, MathHelper.Lerp(initScaleY, 5, quoteant)) / texture.Size();

				Color color = i switch
				{
					0 => GetColor(Subtype) * .4f,
					1 => GetColor(Subtype) * .8f,
					_ => Color.White * 1.5f
				};
				color = (color with { A = 0 }) * (float)(1f - quoteant);

				Main.EntitySpriteDraw(texture, origin - Main.screenPosition, null, color, Projectile.velocity.ToRotation(), new Vector2(0, texture.Height / 2), scale, SpriteEffects.None, 0);

				Texture2D bloom = Mod.Assets.Request<Texture2D>("Effects/Masks/CircleGradient").Value;
				Vector2 endPos = origin + (Vector2.UnitX * ShotLength).RotatedBy(Projectile.velocity.ToRotation());
				Main.spriteBatch.Draw(bloom, endPos - Main.screenPosition, null, color, 0, bloom.Size() / 2, (0.15f - (i * 0.03f)) * (float)(1f - quoteant), SpriteEffects.None, 0);
			}

			if (Counter == 1) //Do fancy dusts
			{
				for (int i = 0; i < (ShotLength / 10); i++)
				{
					Vector2 dustPos = Projectile.Center + new Vector2(Main.rand.NextFloat(ShotLength), Main.rand.NextFloat(-(initScaleY / 4), initScaleY / 4)).RotatedBy(Projectile.velocity.ToRotation());
					Dust dust = Dust.NewDustPerfect(dustPos, Dusts[Main.rand.Next(2)], Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.5f, 1.0f));
					dust.velocity = Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f);
					dust.noGravity = true;
				}
			}

			return false;
		}
	}
}