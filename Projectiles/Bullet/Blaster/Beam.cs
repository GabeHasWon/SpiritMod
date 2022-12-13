using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet.Blaster
{
    public class Beam : SubtypeProj
    {
		private readonly int frameDur = 3;
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
				Vector2 velocity = new Vector2(2f, 0).RotatedBy(Projectile.rotation);
                float speed = velocity.Length();
                int maxRange = maxBeamLength + 30;
                //Test instantaneous collision
                for (int i = 0; i < (int)(maxRange / speed) + 1; i++)
                {
                    Vector2 samplePos = Projectile.position + (velocity * i);
                    if (Collision.CheckAABBvAABBCollision(samplePos, projHitbox.Size(), targetHitbox.TopLeft(), targetHitbox.Size()))
                    {
                        lastStrikePos = samplePos + projHitbox.Size() / 2;
						if (Player == Main.LocalPlayer)
						{
							Projectile.velocity = Main.player[Projectile.owner].DirectionTo(Main.MouseWorld) * ((Projectile.Distance((Vector2)lastStrikePos) / Projectile.timeLeft) - 2);
							Projectile.netUpdate = true;
						}
						return true;
                    }
                }
            }
            return false;
        }

        public override bool? CanDamage() => (Projectile.frame == 0 && Projectile.frameCounter <= 1 && lastStrikePos == null) ? null : false;

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
			//Draw the beam body
			int beamLength = maxBeamLength;
			if (lastStrikePos != null)
				beamLength = (int)((Vector2)lastStrikePos - Projectile.Center).Length();
			int beamSegments = beamLength / rect2.Width;
			for (int i = 0; i < beamSegments; i++)
			{
				texture2 = (i >= (beamSegments - 1)) ? ModContent.Request<Texture2D>(Texture + "_End").Value : TextureAssets.Projectile[Projectile.type].Value;
				origin = new Vector2(0f, rect2.Height / 2);
				var position = Projectile.Center + new Vector2(rect.Width + (rect2.Width * i), 0f).RotatedBy(Projectile.rotation);
				Main.EntitySpriteDraw(texture2, position - Main.screenPosition, rect2, Projectile.GetAlpha(Color.White),
					Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}