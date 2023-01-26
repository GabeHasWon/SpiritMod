using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
    public class Beam : SubtypeProj
	{
		private readonly int frameDur = 3;

		private float beamLength;
		private readonly int maxBeamLength = 300;

		private Vector2? lastStrikePos;

		private Player Player => Main.player[Projectile.owner];

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
			Main.projFrames[Projectile.type] = 3;
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
			if (lastStrikePos != null && Projectile.velocity == Vector2.Zero)
			{
				if (Player == Main.LocalPlayer)
				{
					Projectile.velocity = Main.player[Projectile.owner].DirectionTo(Main.MouseWorld) * 2;
					Projectile.netUpdate = true;
				}
			}

            if (++Projectile.frameCounter >= frameDur)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
			}
        }

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => hitDirection = (target.position.X < Projectile.position.X) ? -1 : 1;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //Only test collision once after firing
            if (CanDamage() != false)
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

				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, lineEnd, 10, ref collisionPoint))
				{
					lastStrikePos = Vector2.Lerp(lineStart, lineEnd, collisionPoint / lineStart.Distance(lineEnd));
					return true;
				}
            }
            return false;
        }

        public override bool? CanDamage() => (Projectile.frame == 0 && Projectile.frameCounter <= 1 && lastStrikePos == null) ? null : false;

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
			int[] beamWidths = new int[]{ 12, 6 };

			Vector2 origin = new Vector2(0f, rect.Height / 2);
			Color drawColor = ColorEffectsIndex.GetColor(Subtype);

			//Draw the beam start
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(drawColor),
				Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(glowTexture, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(Color.White),
				Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			//Draw the beam body
			if (lastStrikePos != null)
				beamLength = (int)((Vector2)lastStrikePos - Projectile.Center).Length();
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