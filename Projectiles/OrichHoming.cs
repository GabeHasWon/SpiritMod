using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.Trails;
using System;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class OrichHoming : ModProjectile, ITrailProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Orichalcum Bloom");

		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 36;
			Projectile.extraUpdates = 1;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
		}

		public void DoTrailCreation(TrailManager tManager) => tManager.CreateTrail(Projectile, new GradientTrail(new Color(182, 66, 245), new Color(105, 42, 168)), new RoundCap(), new DefaultTrailPosition(), 20f, 150f);

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

			Lighting.AddLight((int)(Projectile.position.X / 16f), (int)(Projectile.position.Y / 16f), 0.4f, 0.17f, 0.56f);

			int targetWhoAmI = -1;
			float maxDist = 300f;

			for (int i = 0; i < 200; i++)
			{
				NPC npc = Main.npc[i];

				if (npc.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1) && Projectile.DistanceSQ(npc.Center) < maxDist * maxDist)
				{
					maxDist = Projectile.Distance(npc.Center);
					targetWhoAmI = i;
				}
			}

			if (targetWhoAmI != -1)
			{
				Projectile.timeLeft++;
				Projectile.velocity += Projectile.DirectionTo(Main.npc[targetWhoAmI].Center) * 0.6f;

				const float Magnitude = 5;

				if (Projectile.velocity.LengthSquared() > Magnitude * Magnitude)
					Projectile.velocity = Vector2.Normalize(Projectile.velocity) * Magnitude;
			}

			if (++Projectile.ai[1] >= 7200f)
			{
				Projectile.alpha += 5;
				if (Projectile.alpha > 255)
				{
					Projectile.alpha = 255;
					Projectile.Kill();
				}
			}

			Projectile.rotation += 0.3f;
			Projectile.localAI[0] += 1f;

			if (Projectile.localAI[0] >= 10f)
			{
				Projectile.localAI[0] = 0f;
				int num416 = 0;
				int num417 = 0;
				float num418 = 0f;

				for (int num420 = 0; num420 < Main.maxProjectiles; num420++)
				{
					if (Main.projectile[num420].active && Main.projectile[num420].owner == Projectile.owner && Main.projectile[num420].type == Type && Main.projectile[num420].ai[1] < 3600f)
					{
						num416++;
						if (Main.projectile[num420].ai[1] > num418)
						{
							num417 = num420;
							num418 = Main.projectile[num420].ai[1];
						}
					}
					if (num416 > 8)
					{
						Main.projectile[num417].netUpdate = true;
						Main.projectile[num417].ai[1] = 36000f;
						return;
					}
				}
			}
		}
	}
}