using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.TideDrops;
using Terraria.GameContent.Bestiary;
using SpiritMod.Biomes.Events;

namespace SpiritMod.NPCs.Tides
{
	public class KakamoraParachuter : ModNPC
	{
		public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 3;

		public override void SetDefaults()
		{
			NPC.width = 46;
			NPC.height = 60;
			NPC.damage = 18;
			NPC.defense = 6;
			NPC.lifeMax = 160;
			NPC.noGravity = true;
			NPC.knockBackResist = .9f;
			NPC.value = 200f;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath1;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.KakamoraGliderBanner>();
			SpawnModBiomes = new int[] { ModContent.GetInstance<TideBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		public override void AI()
		{
			bool justSpawned = NPC.ai[3] == 0;
			if (justSpawned)
			{
				NPC.ai[3] = 1;
				NPC.position.Y -= Main.rand.Next(1300, 1700);
				NPC.netUpdate = true;
			}

			NPC.TargetClosest();
			NPC.spriteDirection = NPC.direction;

			NPC.velocity = new Vector2(MathHelper.Lerp(NPC.velocity.X, 4 * NPC.direction, .015f), 1);
			NPC.rotation = NPC.velocity.X * -.08f;

			if (NPC.collideY || (!justSpawned && NPC.velocity.Y == 0))
			{
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(NPC.direction * -4, -.5f), ModContent.ProjectileType<StrayGlider>(), 0, 0);
				
				float lostLife = NPC.life / (float)NPC.lifeMax;
				switch (Main.rand.Next(4))
				{
					case 0:
						NPC.Transform(ModContent.NPCType<KakamoraRunner>());
						break;
					case 1:
						NPC.Transform(ModContent.NPCType<SpearKakamora>());
						break;
					case 2:
						NPC.Transform(ModContent.NPCType<SwordKakamora>());
						break;
					case 3:
						NPC.Transform(ModContent.NPCType<KakamoraShielder>());
						break;
				}
				NPC.life = (int)(NPC.lifeMax * lostLife);
				NPC.netUpdate = true;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<CoconutGun>(50);
			npcLoot.AddCommon<TikiJavelin>(50);
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += .25f;
			NPC.frameCounter %= Main.npcFrameCount[Type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 10; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DynastyWood, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, .7f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DynastyWood, 2.5f * hit.HitDirection, -2.5f, 0, default, .34f);
			}

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Kakamora/KakamoraDeath"), NPC.Center);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Kakamora_Gore").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Kakamora_Gore1").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Kakamora_GoreGlider").Type, 1f);
			}
		}
	}
}