using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet.Blaster
{
    public class BigBeam : SubtypeProj
    {
		private Color commonColor = Color.White;
		private int[] dustType = new int[2];

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

		public override bool PreAI()
		{
			switch (Subtype)
			{
				case 1:
					commonColor = Color.LimeGreen;
					dustType = new int[] { DustID.FartInAJar, DustID.GreenTorch };
					break;
				case 2:
					commonColor = Color.White;
					dustType = new int[] { DustID.FrostHydra, DustID.IceTorch };
					break;
				case 3:
					commonColor = Color.HotPink;
					dustType = new int[] { DustID.Pixie, DustID.PinkTorch };
					break;
				default:
					commonColor = Color.Orange;
					dustType = new int[] { DustID.SolarFlare, DustID.Torch };
					break;
			}
			return true;
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
						Vector2 scale = new Vector2(Main.rand.NextFloat(0.8f, 1.5f), beamLength / 72);
						Vector2 position = lineStart + new Vector2(0, Main.rand.NextFloat(-1.0f, 1.0f) * (float)lineWidth).RotatedBy(velocity.ToRotation());
						ParticleHandler.SpawnParticle(new ImpactLine(position, velocity, commonColor, scale, 24, Projectile));
					}
					int loops = (int)(beamLength / 80) - 1;
					for (int i = 0; i < loops; i++)
						ParticleHandler.SpawnParticle(new PulseCircle(Vector2.Lerp(lineStart, lineEnd, (float)((float)i / loops)), commonColor, 50, 15, PulseCircle.MovementType.OutwardsQuadratic) 
						{ 
							Angle = Projectile.rotation,
							ZRotation = 0.7f,
							Velocity = velocity * 3f
						});
				}
				for (int i = 0; i < 20; i++)
				{
					Dust dust = Dust.NewDustPerfect(lineEnd, dustType[Main.rand.Next(2)], -(velocity * Main.rand.NextFloat(1.0f, 5.0f)).RotatedByRandom(1.5f), 0, default, Main.rand.NextFloat(1.0f, 1.5f));
					dust.noGravity = true;
				}
				spawnedVFX = true;
			}

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, lineEnd, lineWidth, ref collisionPoint);
        }

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Start").Value;
			Texture2D texture2 = TextureAssets.Projectile[Projectile.type].Value;

			Rectangle rect = GetDrawFrame(texture);
			Rectangle rect2 = GetDrawFrame(texture2);

			var origin = new Vector2(0f, rect.Height / 2);
			//Draw the beam start
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(Color.White),
				Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			SpriteEffects effects = SpriteEffects.None;

			//Draw the beam body
			int beamSegments = (int)beamLength / rect2.Width;
			for (int i = 0; i < beamSegments; i++)
			{
				effects = (effects == SpriteEffects.None) ? SpriteEffects.FlipVertically : SpriteEffects.None;

				texture2 = (i >= (beamSegments - 1)) ? ModContent.Request<Texture2D>(Texture + "_End").Value : TextureAssets.Projectile[Projectile.type].Value;
				origin = new Vector2(0f, rect2.Height / 2);
				var position = Projectile.Center + new Vector2(rect.Width + (rect2.Width * i), 0f).RotatedBy(Projectile.rotation);
				Main.EntitySpriteDraw(texture2, position - Main.screenPosition, rect2, Projectile.GetAlpha(Color.White),
					Projectile.rotation, origin, Projectile.scale, effects, 0);
			}
			return false;
		}
	}
}