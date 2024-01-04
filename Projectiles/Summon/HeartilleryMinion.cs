using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Utilities;
using System.Linq;

namespace SpiritMod.Projectiles.Summon
{
	public class HeartilleryMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 9;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 28;
			Projectile.timeLeft = Projectile.SentryLifeTime;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.sentry = true;
			Projectile.ignoreWater = true;
			Projectile.sentry = true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void AI()
		{
			float lerpScale = (Main.mouseTextColor / 200f - 0.35f) * .3f;
			Projectile.scale = lerpScale + .85f;

			if (++Projectile.frameCounter >= 6f)
			{
				if ((Projectile.frame + 1) >= 8)
				{
					var target = Projectile.OwnerMinionAttackTargetNPC ?? Main.npc.Where(x => x.CanBeChasedBy(Projectile) && (x.Distance(Projectile.Center) / 16) < 22).OrderBy(x => x.Distance(Projectile.Center)).FirstOrDefault();
					if (target != default)
					{
						Vector2 vel = ArcVelocityHelper.GetArcVel(Projectile.Center, target.Center, .4325f, 100, heightabovetarget: 20);
						for (int i = 0; i < 25; i++)
						{
							Dust dust = Dust.NewDustDirect(Projectile.Center - Vector2.UnitY * 5, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Blood>(), 0f, -2f, 0, default, .85f);
							dust.velocity = vel.RotatedByRandom(MathHelper.Pi / 14) * Main.rand.NextFloat(0.1f, 0.6f);
						}

						if (Projectile.owner == Main.myPlayer)
						{
							Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<HeartilleryMinionClump>(), Projectile.damage, 0);
							
							int numproj = Main.rand.Next(1, 4);
							for (int i = 0; i < numproj; i++)
								Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel.RotatedByRandom(MathHelper.Pi / 8) * Main.rand.NextFloat(0.9f, 1.1f), ModContent.ProjectileType<HeartilleryMinionClump>(), Projectile.damage, 0).netUpdate = true;
						}
						SoundEngine.PlaySound(SoundID.Item95, Projectile.Center);
					}
				}
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
			}
			else if (Projectile.frameCounter >= 10f)
			{
				Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
				Projectile.frameCounter = 0;
				if (Projectile.frame >= 3)
					Projectile.frame = 0;
			}

            for (int i = 0; i < 3; ++i)
            {
				Vector2 offset = new Vector2(Projectile.velocity.X, -Projectile.velocity.Y) * .2f * i;

                Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.Blood, 0f, .5f, 100, default, .7f);
				dust.noGravity = false;
				dust.position -= offset;
            }
        }

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath22, Projectile.Center);
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, -2f, 0, default, .85f);
				dust.noGravity = false;
				dust.position += new Vector2(Main.rand.Next(-50, 51) * .05f - 1.5f, Main.rand.Next(-50, 51) * .05f - 1.5f);

				if (dust.position != Projectile.Center)
					dust.velocity = Projectile.DirectionTo(dust.position) * 1f;
			}
		}
	}
}