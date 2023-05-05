using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic
{
	public class GhostFeather : ModProjectile, ITrailProjectile, IDrawAdditive
	{
		private float Curve
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public void DoTrailCreation(TrailManager tManager)
		{
			//Misc spawn effects
			Projectile.scale = Main.rand.NextFloat(0.7f, 1.0f);
			Projectile.frame = Main.rand.Next(Main.projFrames[Type]);

			Projectile.netUpdate = true;

			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.LightBlue), new NoCap(), new DefaultTrailPosition(), Projectile.scale * 10f, 80f, new DefaultShader());
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ghost Feather");
			Main.projFrames[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 5;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 2;
			Projectile.timeLeft = 200;
			Projectile.extraUpdates = 1;
			Projectile.alpha = 255;
			Projectile.tileCollide = false;
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			Projectile.alpha = Math.Max(0, Projectile.alpha - (255 / 30));

			int maxRange = 500;
			bool foundNPC = false;

			foreach (NPC npc in Main.npc) //Home in on nearby NPCs
			{
				if (npc.active && !npc.friendly && npc.CanBeChasedBy(Projectile) && Projectile.Distance(npc.Center) <= maxRange)
				{
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(npc.Center) * 10f, 0.025f);
					foundNPC = true;

					break;
				}
			}

			if (!foundNPC)
				Projectile.velocity = Projectile.velocity.RotatedBy(Curve);
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + ((Projectile.direction < 0) ? MathHelper.Pi : 0);

			Projectile.position -= Projectile.velocity * (float)(1f - Projectile.Opacity);

			if (Main.rand.NextBool(4))
			{
				Color color = Color.Lerp(Color.White, Color.Yellow, Main.rand.NextFloat(1.0f));
				Dust dust = Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(8.0f)), DustID.Phantasmal, Projectile.velocity / 5, 80, color, Main.rand.NextFloat(0.5f, 1.0f));
				dust.noGravity = true;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (!Main.dedServ)
				for (int i = 0; i < 2; i++)
					ParticleHandler.SpawnParticle(new ImpactLine(Projectile.Center, Vector2.Normalize(Projectile.velocity), (i == 0) ? Color.SeaGreen : Color.White, Vector2.One - (Vector2.One * i * .25f), 20));
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);
			Vector2 drawOrigin = frame.Size() / 2;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);
			Vector2 drawOrigin = frame.Size() / 2;
			float scale = Projectile.scale + (((float)Main.timeForVisualEffects / 20f).ToRotationVector2().Y * .25f);

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - screenPos + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.White) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				spriteBatch.Draw(texture, drawPos, frame, color, Projectile.oldRot[k], drawOrigin, scale, SpriteEffects.None, 0);
			}
		}
	}
}