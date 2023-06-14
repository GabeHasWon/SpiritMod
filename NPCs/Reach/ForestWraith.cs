using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Consumable;
using SpiritMod.Items.Sets.BriarDrops;
using SpiritMod.Items.Sets.HuskstalkSet;
using SpiritMod.Items.Sets.GladeWraithDrops;
using SpiritMod.Projectiles;
using SpiritMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.Reach
{
	public class ForestWraith : ModNPC, IBCRegistrable
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glade Wraith");
			Main.npcFrameCount[NPC.type] = 6;

			NPCHelper.ImmuneTo(this, BuffID.Poisoned);
			NPCID.Sets.MPAllowedEnemies[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 44;
			NPC.height = 82;
			NPC.damage = 28;
			NPC.defense = 10;
			NPC.lifeMax = 300;
			NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 541f;
			NPC.knockBackResist = 0.05f;
			NPC.noGravity = true;
			NPC.chaseable = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = 44;
			AIType = NPCID.FlyingFish;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.GladeWraithBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.BriarSurfaceBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
			{
				new FlavorTextBestiaryInfoElement("Despite the Briar's unnatural creeping presence, it has left its mark on nature just long enough for these vengeful spirits to call it home."),
			});

			bestiaryEntry.UIInfoProvider = new CustomEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], false, 25);
		}

		bool throwing = false;

		int timer = 0;
		bool thrown = false;

		public override bool PreAI()
		{
			Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.46f, 0.32f, .1f);
			
			timer = ++timer % 750;
			if (timer == 240 || timer == 280 || timer == 320)
			{
				SoundEngine.PlaySound(SoundID.Grass, NPC.Center);
				Vector2 direction = NPC.DirectionTo(Main.player[NPC.target].Center) * 10f;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(0.02f), ModContent.ProjectileType<OvergrowthLeaf>(), 6, 1, Main.myPlayer);

					Main.projectile[p].hostile = true;
					Main.projectile[p].friendly = false;
					Main.projectile[p].minion = false;
				}
				NPC.netUpdate = true;
			}
			if (timer >= 500 && timer <= 720)
			{
				throwing = true;
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, -2.5f, 0, default, 0.6f);
				NPC.defense = 0;
				NPC.velocity = Vector2.Zero;

				if ((int)NPC.frameCounter == 4 && !thrown)
				{
					thrown = true;
					Vector2 direction = NPC.GetArcVel(Main.player[NPC.target].Center, 0.4f, 100, 500, maxXvel: 14);
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

					if (Main.netMode != NetmodeID.MultiplayerClient)
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction, ModContent.ProjectileType<LittleBouncingSpore>(), 8, 1, Main.myPlayer, 0, 0);
				}

				if ((int)NPC.frameCounter != 4)
					thrown = false;

				NPC.netUpdate = true;
			}
			else
				throwing = false;

			if (timer >= 730)
			{
				Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);
				direction.X *= Main.rand.Next(6, 9);
				direction.Y *= Main.rand.Next(6, 9);
				NPC.velocity = direction *= 0.97f;
				timer = 0;

				SoundEngine.PlaySound(SoundID.Zombie7, NPC.Center);
				NPC.defense = 10;
				NPC.netUpdate = true;
			}
			else if (WorldGen.SolidTile(Framing.GetTileSafely(NPC.Center)))
				NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Main.player[NPC.target].Center) * 8f, .05f);

			NPC.spriteDirection = NPC.direction;
			return true;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 30; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GrassBlades, 2.5f * hitDirection, -2.5f, 0, default, 0.3f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WoodFurniture, 2.5f * hitDirection, -2.5f, 0, default, .34f);
			}

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				for (int i = 1; i < 6; i++)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("GladeWraith" + i).Type);

				NPC.position.X = NPC.position.X + (NPC.width / 2);
				NPC.position.Y = NPC.position.Y + (NPC.height / 2);
				NPC.width = 30;
				NPC.height = 30;
				NPC.position.X = NPC.position.X - (NPC.width / 2);
				NPC.position.Y = NPC.position.Y - (NPC.height / 2);

				for (int num621 = 0; num621 < 20; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Plantera_Green, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
					}
				}
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;
			if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust)
				&& ((!Main.pumpkinMoon && !Main.snowMoon) || spawnInfo.SpawnTileY > Main.worldSurface || Main.dayTime)
				&& (!Main.eclipse || spawnInfo.SpawnTileY > Main.worldSurface || !Main.dayTime)
				&& (SpawnCondition.GoblinArmy.Chance == 0))
			{
				if (!NPC.AnyNPCs(ModContent.NPCType<ForestWraith>()))
					return spawnInfo.Player.ZoneBriar() && NPC.downedBoss1 && !Main.dayTime ? .05f : 0f;
			}
			return 0f;
		}

		public override bool PreKill()
		{
			if (Main.netMode != NetmodeID.Server)
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/DownedMiniboss"), NPC.Center);
			MyWorld.downedGladeWraith = true;

			return true;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter = (NPC.frameCounter += 0.15f) % Main.npcFrameCount[Type];

			int frameX = throwing ? 1 : 0;
			int frameY = (int)NPC.frameCounter;
			int frameWidth = 88; //The width of the texture is 176px

			NPC.frame = new Rectangle(frameX * frameWidth, frameY * frameHeight, frameWidth - 2, frameHeight - 2);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/Reach/ForestWraith_Glow").Value, screenPos);

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<OakHeart>(), ModContent.ItemType<HuskstalkStaff>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EnchantedLeaf>(), 1, 5, 7));
		}

		public void RegisterToChecklist(out BossChecklistDataHandler.EntryType entryType, out float progression,
			out string name, out Func<bool> downedCondition, ref BossChecklistDataHandler.BCIDData identificationData,
			ref string spawnInfo, ref string despawnMessage, ref string texture, ref string headTextureOverride,
			ref Func<bool> isAvailable)
		{
			entryType = BossChecklistDataHandler.EntryType.Miniboss;
			progression = 0.8f;
			name = "Glade Wraith";
			downedCondition = () => MyWorld.downedGladeWraith;
			identificationData = new BossChecklistDataHandler.BCIDData(
				new List<int> { ModContent.NPCType<ForestWraith>() },
				new List<int> {
					ModContent.ItemType<GladeWreath>()
				},
				null,
				new List<int> {
					ModContent.ItemType<HuskstalkStaff>(),
					ModContent.ItemType<AncientBark>()
				});
			spawnInfo =
				"Destroy a Bone Altar in the Underground Briar. The Glade Wraith also spawns naturally at nighttime after defeating the Eye of Cthulhu. Alternatively, find a Glade Wreath in Briar Chests and use it in the Briar at any time.";
			texture = "SpiritMod/Textures/BossChecklist/GladeWraithTexture";
			headTextureOverride = "SpiritMod/NPCs/Reach/ForestWraith_Head_Boss";
		}
	}
}