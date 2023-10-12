using SpiritMod.Projectiles.Hostile;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.NPCs.Tides.Tide;
using SpiritMod.Items.Sets.TideDrops;
using Terraria.GameContent.Bestiary;
using SpiritMod.Biomes.Events;

namespace SpiritMod.NPCs.Tides
{
	public class KakamoraShielder : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { Velocity = 1f };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 48;
			NPC.height = 52;
			NPC.damage = 28;
			NPC.defense = 16;
			AIType = NPCID.SnowFlinx;
			NPC.aiStyle = 3;
			NPC.lifeMax = 140;
			NPC.knockBackResist = .2f;
			NPC.value = 200f;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath1;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.KakamoraShielderBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<TideBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		bool blocking = false;
		int blockTimer = 0;

		public override void AI()
		{
			NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];
			var list2 = Main.projectile.Where(x => x.Hitbox.Intersects(NPC.Hitbox));
			foreach (var proj in list2)
			{
				if (proj.type == ModContent.ProjectileType<ShamanBolt>() && proj.active && NPC.life < NPC.lifeMax - 30)
				{
					NPC.life += 30;
					NPC.HealEffect(30, true);
					proj.active = false;
				}
				else if (proj.type == ModContent.ProjectileType<ShamanBolt>() && proj.active && NPC.life > NPC.lifeMax - 30)
				{
					NPC.life += NPC.lifeMax - NPC.life;
					NPC.HealEffect(NPC.lifeMax - NPC.life, true);
					proj.active = false;
				}
			}

			if (NPC.wet)
			{
				NPC.noGravity = true;
				if (NPC.velocity.Y > -7)
					NPC.velocity.Y -= .085f;
				return;
			}
			else
				NPC.noGravity = false;

			blockTimer++;
			if (blockTimer == 200)
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Kakamora/KakamoraHit"), NPC.Center);
			if (blockTimer > 200)
				blocking = true;
			if (blockTimer > 350)
			{
				blocking = false;
				blockTimer = 0;
			}
			if (blocking)
			{
				NPC.aiStyle = 0;
				NPC.velocity.X = 0;
				NPC.noGravity = false;
				NPC.defense = 999;
				NPC.HitSound = SoundID.NPCHit4;
				if (player.position.X > NPC.position.X)
					NPC.spriteDirection = 1;
				else
					NPC.spriteDirection = -1;
			}
			else
			{
				NPC.spriteDirection = NPC.direction;
				NPC.aiStyle = 3;
				NPC.defense = 6;
				NPC.HitSound = SoundID.NPCHit2;
				var list = Main.npc.Where(x => x.Hitbox.Intersects(NPC.Hitbox));
				foreach (var npc2 in list)
				{
					if (npc2.type == ModContent.NPCType<LargeCrustecean>() && NPC.Center.Y > npc2.Center.Y && npc2.active)
					{
						NPC.velocity.X = npc2.direction * 7;
						NPC.velocity.Y = -2;
						SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Kakamora/KakamoraHit"), NPC.Center);
					}
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<CoconutGun>(50);
			npcLoot.AddCommon<TikiJavelin>(50);
		}

		public override void FindFrame(int frameHeight)
		{
			if (((NPC.collideY || NPC.wet) && !blocking) || NPC.IsABestiaryIconDummy)
			{
				NPC.frameCounter += 0.2f;
				NPC.frameCounter %= 4;
				int frame = (int)NPC.frameCounter;
				NPC.frame.Y = frame * frameHeight;
			}

			if (blocking)
				NPC.frame.Y = 4 * frameHeight;
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			if (blocking)
				SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, NPC.position);
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 10; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DynastyWood, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DynastyWood, 2.5f * hit.HitDirection, -2.5f, 0, default, .34f);
			}

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Kakamora/KakamoraDeath"), NPC.Center);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Kakamora_Gore1").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Kakamora_Gore2").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Kakamora_Gore3").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ShielderGore1").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ShielderGore2").Type, 1f);
			}
		}
	}
}
