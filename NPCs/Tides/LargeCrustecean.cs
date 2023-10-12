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
	public class LargeCrustecean : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bubble Brute");
			Main.npcFrameCount[NPC.type] = 9;

			var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f,
				Position = new Vector2(16f, 28f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 0f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 80;
			NPC.height = 82;
			NPC.damage = 25;
			NPC.defense = 4;
			AIType = NPCID.WalkingAntlion;
			NPC.aiStyle = 3;
			NPC.lifeMax = 375;
			NPC.knockBackResist = .2f;
			NPC.value = 200f;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit18;
			NPC.DeathSound = SoundID.NPCDeath5;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BubbleBruteBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<TideBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		bool blocking = false;
		int blockTimer = 0;

		public override void AI()
		{
			NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];
			blockTimer++;
			if (NPC.wet)
			{
				NPC.noGravity = true;
				if (NPC.velocity.Y > -7)
					NPC.velocity.Y -= .085f;
				return;
			}
			else
				NPC.noGravity = false;

			if (blockTimer == 200)
			{
				//   Main.PlaySound(SoundLoader.customSoundType, npc.position, mod.GetSoundSlot(SoundType.Custom, "Sounds/Kakamora/KakamoraThrow"));
				NPC.frameCounter = 0;
				NPC.velocity.X = 0;
			}

			if (blockTimer > 250)
				blocking = true;

			if (blockTimer > 350)
			{
				blocking = false;
				blockTimer = 0;
				NPC.frameCounter = 0;
			}
			if (blocking)
			{
				NPC.aiStyle = 0;

				if (player.position.X > NPC.position.X)
					NPC.spriteDirection = 1;
				else
					NPC.spriteDirection = -1;
			}
			else
			{
				NPC.spriteDirection = NPC.direction;
				NPC.aiStyle = 3;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<PumpBubbleGun>(15);
			npcLoot.AddCommon<TribalScale>(3, 1, 2);
		}

		public override void FindFrame(int frameHeight)
		{
			if (((NPC.collideY || NPC.wet) && !blocking) || NPC.IsABestiaryIconDummy)
			{
				NPC.frameCounter += 0.2f;
				NPC.frameCounter %= 6;
				int frame = (int)NPC.frameCounter;
				NPC.frame.Y = frame * frameHeight;
			}

			if (NPC.wet)
				return;

			if (blocking)
			{
				NPC.frameCounter += 0.05f;
				NPC.frameCounter = MathHelper.Clamp((float)NPC.frameCounter, 0, 2.9f);
				int frame = (int)NPC.frameCounter;
				NPC.frame.Y = (frame + 6) * frameHeight;
				if (NPC.frameCounter > 2 && blockTimer % 5 == 0)
				{
					SoundEngine.PlaySound(SoundID.Item85, NPC.Center);
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + (NPC.direction * 34), NPC.Center.Y - 4, NPC.direction * Main.rand.NextFloat(3, 6), 0 - Main.rand.NextFloat(1), ModContent.ProjectileType<LobsterBubbleSmall>(), NPC.damage / 2, 1, Main.myPlayer, 0, 0);
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 30; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.14f);
			}

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
				for (int i = 1; i < 8; ++i)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("lobster" + i).Type, 1f);
		}
	}
}