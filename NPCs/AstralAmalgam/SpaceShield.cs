using Microsoft.Xna.Framework;
using SpiritMod.NPCs.Shockhopper;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.AstralAmalgam
{
	public class SpaceShield : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Buffer");
			NPCHelper.BuffImmune(Type, true);

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.noTileCollide = true;
			NPC.width = 32;
			NPC.height = 32;
			NPC.netAlways = true;
			NPC.damage = 15;
			NPC.defense = 9999;
			NPC.alpha = 255;
			NPC.npcSlots = 0;
			NPC.dontTakeDamage = true;
			NPC.lifeMax = 100;
			NPC.friendly = false;
			NPC.chaseable = false;
			NPC.noGravity = true;
			NPC.knockBackResist = 0f;
			NPC.dontCountMe = true;
		}

		public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
		{
			NPC.dontTakeDamage = true;

			if (!projectile.minion && !projectile.sentry && !Main.player[projectile.owner].channel)
			{
				projectile.damage = (int)(projectile.damage * 0.8f);

				if (++NPC.ai[0] > 2)
				{
					NPC.ai[0] = 0;

					SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
					int close = Player.FindClosest(NPC.position, NPC.width, NPC.height);
					Projectile.NewProjectile(NPC.GetSource_OnHurt(projectile), NPC.Center, NPC.DirectionTo(Main.player[close].Center) * 25, ModContent.ProjectileType<HopperLaser>(), 25, 1, Main.myPlayer);
				}
			}
			NPC.life = 100;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			NPC.dontTakeDamage = true;
			NPC.life = 100;

			if (!NPC.active)
			{
				for (int i = 0; i < 20; i++)
				{
					int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedWhiteBluePurple, 0f, -2f, 0, default, 1f);
					Main.dust[num].noGravity = true;
					Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
					if (Main.dust[num].position != NPC.Center)
						Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
				}
			}
		}

		public override void OnKill() => SoundEngine.PlaySound(SoundID.Item14, NPC.position);
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

		public override bool PreAI()
		{
			NPC.life = 100;
			NPC.dontTakeDamage = false;

			float num8 = Main.LocalPlayer.miscCounter / 60f;
			float num7 = 1.0471975512f;

			for (int i = 0; i < 6; i++)
			{
				int num6 = Dust.NewDust(NPC.Center, 0, 0, DustID.DungeonSpirit, 0f, 0f, 100, default, 1.3f);
				Main.dust[num6].noGravity = true;
				Main.dust[num6].velocity = Vector2.Zero;
				Main.dust[num6].noLight = true;
				Main.dust[num6].position = NPC.Center + (num8 * 6.28318548f + num7 * i).ToRotationVector2() * 12f;
			}

			NPC.rotation += 3f;

			if (NPC.ai[3] < (double)Main.npc.Length)
			{
				NPC parent = Main.npc[(int)NPC.ai[3]];

				//Factors for calculations
				double deg = NPC.ai[1]; //The degrees, you can multiply npc.ai[1] to make it orbit faster, may be choppy depending on the value
				double rad = deg * (Math.PI / 180); //Convert degrees to radians
				double dist = 60; //Distance away from the npc

				//Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
				NPC.ai[1] += 1.2f;

				NPC.position.X = parent.Center.X - (int)(Math.Cos(rad) * dist) - NPC.width / 2;
				NPC.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * dist) - NPC.height / 2;
				if (!parent.active || parent.type != ModContent.NPCType<AstralAmalgam>())
				{
					NPC.life = 0;
					NPC.HitEffect(0, 10);
					NPC.active = false;
				}
			}
			return false;
		}
	}
}
