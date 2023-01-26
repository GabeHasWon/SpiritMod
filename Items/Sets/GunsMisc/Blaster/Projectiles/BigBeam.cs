using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
    public class BigBeam : SubtypeProj
    {
		private readonly int frameDur = 3;

		private float beamLength;
		private readonly int maxBeamLength = 1200;

		private bool spawnedVFX = false;

		private Player Player => Main.player[Projectile.owner];

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
			Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

		public override void OnSpawn(IEntitySource source)
		{
			if (Player != Main.LocalPlayer)
				return;

			Projectile.timeLeft = frameDur * Main.projFrames[Projectile.type];

			Projectile.velocity = Player.DirectionTo(Main.MouseWorld) * 2;
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X < 0) ? -1 : 1;

			Projectile.Center = Player.Center + new Vector2(30f, -6 * Projectile.direction).RotatedBy(Projectile.rotation);
			Projectile.velocity = Vector2.Zero;

			Projectile.netUpdate = true;
		}

		public override void AI()
        {
            if (++Projectile.frameCounter >= frameDur)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
			}
        }

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => hitDirection = (target.position.X < Projectile.position.X) ? -1 : 1;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			Vector2 velocity = Vector2.Normalize(new Vector2(1f, 0).RotatedBy(Projectile.rotation));
			int maxRange = maxBeamLength + 30;

			float collisionPoint = 0;
			Vector2 lineStart = Projectile.Center;

			float[] samples = new float[4];
			Collision.LaserScan(lineStart, velocity, 1, maxRange, samples);
			beamLength = 0;
			foreach (float sample in samples)
				beamLength += sample / samples.Length;

			Vector2 lineEnd = Projectile.Center + (velocity * beamLength);
			float lineWidth = 10;

			if (!spawnedVFX)
			{
				if (!Main.dedServ)
				{
					for (int i = 0; i < 6; i++)
					{
						Vector2 scale = new Vector2(Main.rand.NextFloat(0.5f, 1.0f), beamLength / 64);
						Vector2 position = lineStart + new Vector2(0, Main.rand.NextFloat(-1.0f, 1.0f) * (float)lineWidth).RotatedBy(velocity.ToRotation());
						ParticleHandler.SpawnParticle(new ImpactLine(position, velocity, ColorEffectsIndex.GetColor(Subtype), scale, 24, Projectile));
					}
					int loops = (int)(beamLength / 80) - 1;
					for (int i = 0; i < loops; i++)
						ParticleHandler.SpawnParticle(new PulseCircle(Vector2.Lerp(lineStart, lineEnd, (float)((float)i / loops)), ColorEffectsIndex.GetColor(Subtype), 50, 15, PulseCircle.MovementType.OutwardsQuadratic) 
						{ 
							Angle = Projectile.rotation,
							ZRotation = 0.7f,
							Velocity = velocity * 3f
						});
				}
				for (int i = 0; i < 30; i++)
				{
					int[] dustType = ColorEffectsIndex.GetDusts(Subtype);
					Vector2 dustVel = (i < 10) ? (velocity * Main.rand.NextFloat(0.5f, 3.0f)).RotatedBy(Main.rand.NextFromList(-1.57f, 1.57f)) : -(velocity * Main.rand.NextFloat(1.0f, 5.0f)).RotatedByRandom(1.5f);

					Dust dust = Dust.NewDustPerfect(lineEnd, dustType[Main.rand.Next(dustType.Length)], dustVel, 0, default, Main.rand.NextFloat(1.0f, 1.5f));

					dust.fadeIn = 1f;
					dust.noGravity = true;
				}
				spawnedVFX = true;
			}

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, lineEnd, lineWidth, ref collisionPoint);
        }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			int? debuffType = ColorEffectsIndex.GetDebuffs(Subtype);
			if (debuffType != null)
				target.AddBuff(debuffType.Value, 200);
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

			int vFrames = 3;
			Rectangle rect = new(0, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, texture.Width / vFrames, texture.Height / Main.projFrames[Projectile.type]);
			int[] beamWidths = new int[]{ 26, 10 };

			Vector2 origin = new Vector2(0f, rect.Height / 2);
			Color drawColor = ColorEffectsIndex.GetColor(Subtype);

			//Draw the beam start
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(drawColor),
				Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(glowTexture, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(Color.White),
				Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			//Draw the beam body
			int beamSegments = (int)beamLength / beamWidths[1];
			for (int i = 0; i < beamSegments; i++)
			{
				rect.X = texture.Width / vFrames * ((i >= (beamSegments - 1)) ? 2 : 1);
				var position = Projectile.Center + new Vector2(beamWidths[0] + (beamWidths[1] * i), 0f).RotatedBy(Projectile.rotation);

				Main.EntitySpriteDraw(texture, position - Main.screenPosition, rect, Projectile.GetAlpha(drawColor),
					Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
				Main.EntitySpriteDraw(glowTexture, position - Main.screenPosition, rect, Projectile.GetAlpha(Color.White),
					Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}