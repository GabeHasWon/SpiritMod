using Microsoft.Xna.Framework;
using SpiritMod.GlobalClasses.Players;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Glyph
{
	public class PhaseShield : ModProjectile
	{
		private float Charge => Main.player[Projectile.owner].GetModPlayer<GlyphPlayer>().veilCounter;
		private float Smoothing { get => Projectile.velocity.X; set => Projectile.velocity.X = value; }

		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 5;
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(28);
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.aiStyle = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 2;
		}

        public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			if (!owner.dead && owner.active && Charge > 0)
			{
				Projectile.timeLeft = 2;
				Projectile.alpha = (int)MathHelper.Lerp(255, 0, Charge);
				Projectile.Center = owner.Center + new Vector2(Smoothing * 16f, (float)(Math.Sin(Main.timeForVisualEffects / 50f) * 5f) + 4);
				Projectile.frame = (int)Math.Abs(Smoothing * Main.projFrames[Type]);
				Projectile.gfxOffY = owner.gfxOffY;

				float oldSmoothing = Smoothing;
				Smoothing = MathHelper.Lerp(Smoothing, owner.direction, .1f);
				Projectile.direction = Projectile.spriteDirection = Smoothing > 0 ? 1 : -1;

				if (Main.rand.NextFloat() < Math.Abs(Smoothing - oldSmoothing))
				{
					Vector2 vel = new Vector2(((Smoothing - oldSmoothing) < 0) ? 1 : -1, 0);
					for (int i = 0; i < 3; i++)
					{
						Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Green, 0, 0, 100, default, .75f);
						dust.noGravity = true;
						dust.velocity = vel;
					}
				}
			}
			else Projectile.Kill();
		}

		public override void Kill(int timeLeft)
		{
			if (Projectile.alpha < 150)
				for (int i = 0; i < 18; i++)
					Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Green, 0, 0, Projectile.alpha, default, .75f).velocity = Vector2.Zero;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
			=> overPlayers.Add(index);

		public override bool? CanCutTiles() => false;

		public override bool? CanDamage() => false;

		public override bool ShouldUpdatePosition() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.QuickDraw(drawColor: Color.White with { A = 0 });
			Projectile.QuickDrawTrail(drawColor: Color.White with { A = 0 });
			return false;
		}
	}
}