using Microsoft.Xna.Framework;
using SpiritMod.Items.Sets.EvilBiomeDrops.GastricGusher;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Bloater
{
	public class Spewer : ModNPC
	{
		public const int COOLDOWN = 180;

		public ref float Counter => ref NPC.ai[1];

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 9;
			NPCHelper.ImmuneTo(this, BuffID.Poisoned, BuffID.Confused);
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 44;
			NPC.damage = 25;
			NPC.defense = 5;
			NPC.knockBackResist = 0.2f;
			NPC.value = 90;
			NPC.lifeMax = 45;
			NPC.HitSound = SoundID.NPCHit18;
			NPC.DeathSound = SoundID.NPCDeath21;
			NPC.noGravity = true;
			NPC.noTileCollide = false;

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BloaterBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "TheCrimson");

		public override void FindFrame(int frameHeight)
		{
			if (Counter > COOLDOWN)
			{
				if (NPC.frameCounter >= 7)
				{
					if ((NPC.frameCounter += .2f) >= Main.npcFrameCount[Type])
						NPC.frameCounter = 7;
				}
				else 
					NPC.frameCounter = (NPC.frameCounter + .2f) % Main.npcFrameCount[Type];
			}
			else 
				NPC.frameCounter = (NPC.frameCounter + .2f) % 4;

			NPC.frame.Y = frameHeight * (int)NPC.frameCounter;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneOverworldHeight && 
			!Main.eclipse && !spawnInfo.PlayerInTown ? .075f : 0f;

		public override void AI()
		{
			NPC.TargetClosest(true);
			var target = Main.player[NPC.target];
			float distance = NPC.Distance(target.Center);

			if (Counter > COOLDOWN)
			{
				NPC.scale = 1f + (float)(Math.Sin(Main.timeForVisualEffects / 2f) * ((Counter - COOLDOWN) / 90f) * .07f);

				var pos = NPC.Center - new Vector2((NPC.direction == -1) ? 20 : 0, 0);
				Dust.NewDustDirect(pos, 20, 20, Main.rand.NextFromList(DustID.Blood, DustID.SomethingRed), 0, .3f, 0, default, (Counter - COOLDOWN) / 70f);

				if (Counter == (COOLDOWN + 1))
					SoundEngine.PlaySound(SoundID.Zombie40, NPC.Center); //Play a warning sound before the attack
				if (Counter >= (COOLDOWN + 40))
				{
					if (Counter == (COOLDOWN + 40))
						SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
					if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(10))
					{
						SoundEngine.PlaySound(SoundID.Item34, NPC.Center);

						Vector2 dir = (NPC.DirectionTo(target.Center) * new Vector2(9.2f, 6.8f + Main.rand.NextFloat(-1f, 1f))).RotatedByRandom(.3f);
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 4), dir, ModContent.ProjectileType<VomitProj>(), NPCUtils.ToActualDamage(26, 1.5f, 2f), 1);
					}
					if (Counter > (COOLDOWN + 100))
						Counter = 0;

					NPC.velocity = NPC.DirectionTo(target.Center) * -.5f;
				}
				else NPC.velocity *= .9f;
			}
			else if (!target.dead)
			{
				float amount = (distance < 120) ? .005f : .05f;
				NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.Center) * MathHelper.Clamp(NPC.Distance(target.Center) / 100, 0, 5), amount);
			}
			if (Counter != COOLDOWN || ((distance < 240) && Collision.CanHit(NPC, target)))
				Counter++;

			if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(30))
			{
				NPC.velocity.Y = NPC.velocity.Y + 0.439f * Main.rand.NextFloatDirection();
				NPC.netUpdate = true;
			}
			if (distance < 120)
			{
				if (NPC.ai[0] > 0f)
					NPC.velocity.Y = NPC.velocity.Y + .039f;
				else
					NPC.velocity.Y = NPC.velocity.Y - .019f;

				if (NPC.ai[0] < -100f || NPC.ai[0] > 100f)
					NPC.velocity.X = NPC.velocity.X + 0.029f;
				else
					NPC.velocity.X = NPC.velocity.X - 0.029f;

				if ((NPC.ai[0] += 0.9f) > 25f)
					NPC.ai[0] = -200f;
			}
			if (Main.rand.NextFloat() < .131579f)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height + 10, DustID.Blood, 0, 1f, 0, new Color(), .7f).velocity *= .1f;

			NPC.noTileCollide = distance > 500;
			NPC.spriteDirection = NPC.direction;
			NPC.rotation = NPC.velocity.X * .08f;
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 23; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection * 1.5f, -1f, 0, default, .91f);

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				SoundEngine.PlaySound(SoundID.NPCDeath30, NPC.Center);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Spewer1").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Spewer2").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Spewer3").Type, 1f);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GastricGusher>(), 25));
		}
	}
}