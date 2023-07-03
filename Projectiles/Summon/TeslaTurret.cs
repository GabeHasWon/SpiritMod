using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon
{
	public class TeslaTurret : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Tesla Turret");
			Main.projFrames[Type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 38;
			Projectile.height = 56;
			Projectile.timeLeft = Projectile.SentryLifeTime;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.sentry = true;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Projectile.velocity.Y = 5;
			//CONFIG INFO
			int range = 30;   //How many tiles away the projectile targets NPCs
			float shootVelocity = 18f; //magnitude of the shoot vector (speed of arrows shot)
			int shootSpeed = 20;

			//TARGET NEAREST NPC WITHIN RANGE
			float lowestDist = float.MaxValue;
			for (int i = 0; i < 200; ++i) {
				NPC npc = Main.npc[i];
				//if npc is a valid target (active, not friendly, and not a critter)
				if (npc.active && npc.CanBeChasedBy(Projectile) && !npc.friendly && !NPCID.Sets.CountsAsCritter[npc.type]) {
					//if npc is within 50 blocks
					float dist = Projectile.Distance(npc.Center);
					if (dist / 16 < range) {
						//if npc is closer than closest found npc
						if (dist < lowestDist) {
							lowestDist = dist;

							//target this npc
							Projectile.ai[1] = npc.whoAmI;
						}
					}
				}
			}

			if (Projectile.ai[1] == -1)
				return;

			NPC target = Main.npc[(int)Projectile.ai[1]]; //our target

			if (!target.active || !target.CanBeChasedBy(Projectile) || target.friendly || NPCID.Sets.CountsAsCritter[target.type])
			{
				target = null;
				Projectile.ai[1] = -1;
			}

			if (target is null)
				return;

			Projectile.ai[0]++;
			if (Projectile.ai[0] % shootSpeed == 4 && target.active && Projectile.Distance(target.Center) / 16 < range)
			{
				Vector2 ShootArea = new Vector2(Projectile.Center.X, Projectile.Center.Y - 25);
				Vector2 direction = Vector2.Normalize(target.Center - ShootArea) * shootVelocity;
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 25, direction.X, direction.Y, ModContent.ProjectileType<TeslaSpikeProjectile>(), Projectile.damage, 0, Main.myPlayer);
				SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}
	}
}