using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Summon;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon
{
	[AutoloadMinionBuff("Mango Jelly", "A mini mango jelly fights for you!")]
	public class MangoJellyMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mango Jelly");
			Main.projFrames[Type] = 4;
			Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[Type] = true;
			ProjectileID.Sets.CultistIsResistantTo[Type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.netImportant = true;
			Projectile.width = 22;
			Projectile.height = 23;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.minionSlots = 1;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 18000;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			AIType = ProjectileID.Raven;
		}

		bool jump = false;
		int xoffset = 0;
		public override Color? GetAlpha(Color lightColor) => new Color(250, 210, 230);

		public override void AI()
		{
			float num = 1f - (float)Projectile.alpha / 255f;
			num *= Projectile.scale;
			float num395 = Main.mouseTextColor / 155f - 0.35f;
			num395 *= 0.34f;
			Projectile.scale = num395 + 0.55f;

			Player player = Main.player[Projectile.owner];
			if (Projectile.Distance(player.Center) > 1500) {
				Projectile.position = player.position + new Vector2(Main.rand.Next(-125, 126), Main.rand.Next(-125, 126));
				for (int i = 0; i < 25; i++) {
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WitherLightning);
				}
			}

			int range = 40;   //How many tiles away the projectile targets NPCs
			float lowestDist = float.MaxValue;
			if (player.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float dist = Projectile.Distance(npc.Center);
				if (dist / 16 < range) {
					Projectile.ai[1] = npc.whoAmI;
				}
			}
			else
			{
				foreach (NPC npc in Main.npc) {
					//if npc is a valid target (active, not friendly, and not a critter)
					if (npc.active && !npc.friendly && npc.catchItem == 0 && npc.CanBeChasedBy(Projectile, false)) {
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
			}
			NPC target = (Main.npc[(int)Projectile.ai[1]] ?? new NPC()); //our target
			if (target.active && !target.friendly && target.type != NPCID.TargetDummy && target.type != NPCID.DD2LanePortal) {
				if (target.position.X > Projectile.position.X) {
					xoffset = Main.rand.Next(24, 28);
				}
				else {
					xoffset = Main.rand.Next(-28, -24);
				}
				Projectile.ai[0]++;
				Projectile.velocity.X *= 0.99f;
				if (!jump) {
					if (Projectile.velocity.Y < 7.5f) {
						Projectile.velocity.Y += 0.095f;
					}
					if (target.position.Y < Projectile.position.Y && Projectile.ai[0] % 8 == 0) {
						jump = true;
						Projectile.velocity.X = xoffset / 1.75f;
						if (target.position.Y < Projectile.position.Y - 150) {
							Projectile.velocity.Y = -9;
						}
						else {
							Projectile.velocity.Y = -1.5f;
						}
					}
					Projectile.rotation = 0f;
				}
				if (jump) {
					Projectile.velocity *= 0.96f;
					if (Math.Abs(Projectile.velocity.X) < 0.9f) {
						jump = false;
					}
					Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
				}
			}
			else {
				if (player.position.X > Projectile.position.X) {
					xoffset = Main.rand.Next(16, 24);
				}
				else {
					xoffset = Main.rand.Next(-24, -16);
				}
				Projectile.ai[0]++;
				Projectile.velocity.X *= 0.99f;
				if (!jump) {
					if (Projectile.velocity.Y < 7.5f) {
						Projectile.velocity.Y += 0.05f;
					}
					if (player.position.Y < Projectile.position.Y && Projectile.ai[0] % 20 == 0) {
						jump = true;
						Projectile.velocity.X = xoffset / 1.25f;
						if (player.position.Y < Projectile.position.Y - 150) {
							Projectile.velocity.Y = -9;
						}
						else {
							Projectile.velocity.Y = -1.5f;
						}
					}
					Projectile.rotation = 0f;
				}
				if (jump) {
					Projectile.velocity *= 0.97f;
					if (Math.Abs(Projectile.velocity.X) < 0.3f) {
						jump = false;
					}
					Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
				}
			}
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 7) {
				Projectile.frame++;
				Projectile.frameCounter = 0;
				if (Projectile.frame >= 4)
					Projectile.frame = 0;
			}
		}

		public override bool MinionContactDamage() => true;
	}
}