using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class Pumpkin : ModProjectile
	{
		int timer = 0;
		bool launch = false;

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.light = 0.5f;
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.timeLeft = 180;
			Projectile.friendly = true;
			Projectile.damage = 10;
			Projectile.alpha = 255;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			for (int num623 = 0; num623 < 25; num623++)
			{
				int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Pumpkin, 0f, 0f, 100, default, 2f);
				Main.dust[num622].noGravity = true;
				Main.dust[num622].velocity *= 1.5f;
				Main.dust[num622].scale = 0.8f;
			}
		}

		public override void AI()
		{
			Projectile.alpha = Math.Clamp(Projectile.alpha - 10, 0, byte.MaxValue);
			timer++;

			Vector2 targetPos = Projectile.Center;
			float targetDist = 1000f;
			bool targetAcquired = false;

			//loop through first 200 NPCs in Main.npc
			//this loop finds the closest valid target NPC within the range of targetDist pixels
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].CanBeChasedBy(Projectile) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
				{
					float dist = Projectile.Distance(Main.npc[i].Center);
					if (dist < targetDist)
					{
						targetDist = dist;
						targetPos = Main.npc[i].Center;
						targetAcquired = true;
					}
				}
			}

			//change trajectory to home in on target
			Projectile.rotation += 0.1f;
			if (timer > 60)
			{
				Projectile.rotation += 0.2f;
				int num622 = Dust.NewDust(new Vector2(Projectile.position.X - Projectile.velocity.X, Projectile.position.Y - Projectile.velocity.Y), Projectile.width, Projectile.height, DustID.Pumpkin, 0f, 0f, 100, default, 2f);
				Main.dust[num622].noGravity = true;
				Main.dust[num622].scale = 0.5f;
			}

			if (targetAcquired && timer > 90 && !launch)
			{
				float homingSpeedFactor = 15f;
				Vector2 homingVect = targetPos - Projectile.Center;
				homingVect.Normalize();
				homingVect *= homingSpeedFactor;

				Projectile.velocity = homingVect;
				launch = true;
			}

			Lighting.AddLight((int)(Projectile.position.X / 16f), (int)(Projectile.position.Y / 16f), 0.749019608f, 0.443137255f, 0.203921569f);
		}
	}
}