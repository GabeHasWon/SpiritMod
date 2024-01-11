using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Consumable.Food;
using SpiritMod.Items.Sets.EvilBiomeDrops.Heartillery;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;

namespace SpiritMod.NPCs.ArterialGrasper
{
	public class CrimsonTrapper : ModNPC
	{
		public ref float Counter => ref NPC.ai[0];

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 4;
			NPCHelper.ImmuneTo(this, BuffID.Poisoned, BuffID.Confused);

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f,
				Position = new(0, 10f)
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 34;
			NPC.height = 34;
			NPC.damage = 35;
			NPC.defense = 8;
			NPC.lifeMax = 150;
			NPC.noGravity = true;
			NPC.HitSound = SoundID.NPCHit19;
			NPC.DeathSound = SoundID.DD2_BetsyHurt;
			NPC.value = 220f;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0f;
			NPC.behindTiles = true;

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.ArterialGrasperBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "TheCrimson");

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */ => NPC.lifeMax = 250;

		public override void OnSpawn(IEntitySource source)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				for (int i = 0; i < Main.rand.Next(2, 4); i++)
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y - 10, Main.rand.Next(-10, 10), -6, ModContent.ProjectileType<TendonEffect>(), 0, 0, ai0: NPC.whoAmI);
				for (int i = 0; i < Main.rand.Next(2, 3); i++)
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y - 10, Main.rand.Next(-10, 10), -6, ModContent.ProjectileType<TendonEffect1>(), 0, 0, ai0: NPC.whoAmI);
			}
		}

		public override void AI()
		{
			NPC.TargetClosest(false);
			Player target = Main.player[NPC.target];
			NPC.spriteDirection = NPC.direction;
			NPC.gfxOffY = (float)(Math.Sin(Main.timeForVisualEffects / 30) * 2f);

			if ((NPC.Distance(target.Center) < 560) || Counter > 180)
				Counter++;
			if (Counter > 220)
			{
				Counter = 0;
				SoundEngine.PlaySound(SoundID.Item95, NPC.Center);
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/HeartbeatFx"), NPC.Center);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					for (int i = 0; i < 5; i++)
					{
						float rotation = (float)(Main.rand.Next(0, 361) * (Math.PI / 180));
						Vector2 velocity = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * 5;
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, velocity.X, velocity.Y, ModContent.ProjectileType<ArterialBloodClump>(), 12, 1);
					}
				}
			}

			NPC.scale = MathHelper.Min(NPC.scale + .025f, 1f);
			if (Main.timeForVisualEffects % 30 == 0)
			{
				Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), .153f, .028f, .055f);
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/HeartbeatFx"), NPC.Center);
				NPC.scale = .9f;
			}
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += .15f;
			NPC.frameCounter %= Main.npcFrameCount[Type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			bool wall = Framing.GetTileSafely(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY).WallType > 0;
			bool valid = wall && spawnInfo.Player.ZoneCrimson && (spawnInfo.Player.ZoneRockLayerHeight || spawnInfo.Player.ZoneDirtLayerHeight);
			if (!valid)
				return 0;
			return SpawnCondition.Crimson.Chance * .1f;
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 30; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, Color.Purple, 0.3f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, default, .34f);
			}

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Grasper1").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Grasper2").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Grasper3").Type, 1f);
			}
		}

		//public override void OnKill()
		//{
		//	if (QuestManager.GetQuest<StylistQuestCrimson>().IsActive)
		//		Item.NewItem(NPC.Center, ModContent.ItemType<Items.Sets.MaterialsMisc.QuestItems.CrimsonDyeMaterial>());
		//}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HeartilleryBeacon>(), 33));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Meatballs>(), 16));
		}
	}
}