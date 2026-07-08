using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using SpiritMod.Items.Halloween;
using SpiritMod.NPCs.Critters.Algae;
using SpiritMod.NPCs.Town;
using SpiritMod.Projectiles.Arrow;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Projectiles.Summon.SacrificialDagger;
using Terraria.Audio;
using SpiritMod.Buffs.DoT;
using SpiritMod.World;
using SpiritMod.NPCs.BlueMoon.Bloomshroom;
using SpiritMod.NPCs.BlueMoon.Glitterfly;
using SpiritMod.NPCs.BlueMoon.GlowToad;
using SpiritMod.NPCs.BlueMoon.Lumantis;
using SpiritMod.NPCs.BlueMoon.MadHatter;
using SpiritMod.NPCs.BlueMoon.LunarSlime;
using SpiritMod.Buffs.Pet;
using SpiritMod.Utilities;
using Terraria.DataStructures;
using SpiritMod.Systems.LuminousOcean;
using Terraria.Localization;

namespace SpiritMod.NPCs
{
	public class GNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		#region Fields
		public int fireStacks;
		public int nebulaFlameStacks;
		public int GhostJellyStacks;
		public int angelLightStacks;
		public int angelWrathStacks;
		public int titanicSetStacks;
		public int acidBurnStacks;
		public bool vineTrap = false;
		public bool clatterPierce = false;
		public bool tracked = false;

		public int summonTag;
		public bool sacrificialDaggerBuff;

		public bool Stopped = false;
		public bool afflicted = false;
		public bool starDestiny = false;
		public int bloodInfusion = 0;
		public bool bloodInfused = false;
		public bool death = false;
		public bool iceCrush = false;

		public int oakHeartStacks;
		public readonly int oakHeartStacksMax = 3;

		public bool doomDestiny = false;

		public bool sFracture = false;
		public bool blaze = false;
		public bool shadowbroken = false;
		public bool impaled = false;

		public float slowDegree;
		private float slowAmt;
		public bool BeingSlowed => slowDegree > 0;
		#endregion

		public override void ResetEffects(NPC npc)
		{
			bloodInfused = false;
			vineTrap = false;
			clatterPierce = false;
			doomDestiny = false;
			sFracture = false;
			death = false;
			starDestiny = false;
			afflicted = false;
			Stopped = false;
			blaze = false;
			tracked = false;
			iceCrush = false;
			shadowbroken = false;
			impaled = false;

			summonTag = 0;
			sacrificialDaggerBuff = false;
			slowDegree = 0;
		}

		public override bool PreAI(NPC npc)
		{
			if (Main.netMode != NetmodeID.Server)
			{
				if (bloodInfusion > 150)
				{
					bloodInfusion = 0;
					Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, 0, 0, ModContent.ProjectileType<FlayedExplosion>(), 25, 0, Main.myPlayer);
				}
			}

			Player player = Main.LocalPlayer;
			MyPlayer modPlayer = player.GetSpiritPlayer();
			Vector2 dist = npc.position - player.position;
			if (Main.netMode != NetmodeID.Server)
			{
				if (player.GetModPlayer<MyPlayer>().HellGaze == true && Math.Sqrt((dist.X * dist.X) + (dist.Y * dist.Y)) < 400 && Main.rand.NextBool(80) && !npc.friendly)
					npc.AddBuff(BuffID.OnFire3, 300, false);
				dist = npc.Center - new Vector2(modPlayer.clockX, modPlayer.clockY);
				if (player.GetModPlayer<MyPlayer>().clockActive == true && Math.Sqrt((dist.X * dist.X) + (dist.Y * dist.Y)) < 175 && !npc.friendly)
					npc.AddBuff(ModContent.BuffType<Stopped>(), 3);
			}

			if (Main.netMode != NetmodeID.Server)
			{
				if (Stopped)
				{
					if (!npc.boss)
					{
						npc.velocity *= 0;
						npc.frame.Y = 0;
						return false;
					}
				}
			}

			if (oakHeartStacks > 0)
				oakHeartStacks--;
			if (impaled)
				npc.velocity = new Vector2(0, 1);

			if (BeingSlowed)
			{
				if ((slowAmt += slowDegree) >= 1)
				{
					slowAmt--;
					return true;
				}
				return false;
			}

			return true;
		}

		public override void PostAI(NPC npc)
		{
			if (BeingSlowed)
				npc.position -= npc.velocity * (float)(1f - slowAmt) * (npc.boss ? 0.25f : 1f);
		}

		public override void HitEffect(NPC npc, NPC.HitInfo hit)
		{
			if ((npc.type == NPCID.GraniteFlyer || npc.type == NPCID.GraniteGolem) && NPC.downedBoss2 && Main.netMode != NetmodeID.MultiplayerClient && npc.life <= 0 && Main.rand.NextBool(3))
			{
				SoundEngine.PlaySound(SoundID.Item109);
				for (int i = 0; i < 20; i++)
				{
					int num = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Electric, 0f, -2f, 0, default, 2f);
					Main.dust[num].noGravity = true;
					Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[num].scale *= .25f;
					if (Main.dust[num].position != npc.Center)
						Main.dust[num].velocity = npc.DirectionTo(Main.dust[num].position) * 6f;
				}
				Vector2 spawnAt = npc.Center + new Vector2(0f, npc.height / 2f);
				NPC.NewNPC(npc.GetSource_OnHurt(null), (int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<NPCs.CracklingCore.GraniteCore>());
			}
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			int before = npc.lifeRegen;
			bool drain = false;
			bool noDamage = damage <= 1;
			int damageBefore = damage;
			if (angelLightStacks > 0)
			{
				if (npc.FindBuffIndex(ModContent.BuffType<AngelLight>()) < 0)
				{
					angelLightStacks = 0;
					return;
				}
			}
			if (angelWrathStacks > 0)
			{
				if (npc.FindBuffIndex(ModContent.BuffType<AngelWrath>()) < 0)
				{
					angelWrathStacks = 0;
					return;
				}
			}

			#region Iriazul
			if (fireStacks > 0)
			{
				if (npc.FindBuffIndex(ModContent.BuffType<StackingFireBuff>()) < 0)
				{
					fireStacks = 0;
					return;
				}

				drain = true;
				npc.lifeRegen -= 16;
				damage = Math.Max(damage, fireStacks * 5);
			}
			if (acidBurnStacks > 0)
			{
				if (npc.FindBuffIndex(ModContent.BuffType<AcidBurn>()) < 0)
				{
					acidBurnStacks = 0;
					return;
				}

				drain = true;
				npc.lifeRegen -= 3 * acidBurnStacks;
				damage = Math.Max(damage, acidBurnStacks * 2);
			}
			if (nebulaFlameStacks > 0)
			{
				if (npc.FindBuffIndex(ModContent.BuffType<NebulaFlame>()) < 0)
				{
					nebulaFlameStacks = 0;
					return;
				}

				drain = true;
				npc.lifeRegen -= 16;
				damage = Math.Max(damage, fireStacks * 20);
			}
			#endregion

			if (doomDestiny)
			{
				drain = true;
				npc.lifeRegen -= 16;
				if (damage < 10)
					damage = 10;
			}

			if (starDestiny)
			{
				drain = true;
				npc.lifeRegen -= 150;
				damage = 75;
			}

			if (sFracture)
			{
				drain = true;
				npc.lifeRegen -= 9;
				damage = 3;
			}

			if (npc.HasBuff<SoulBurn>())
			{
				drain = true;
				npc.lifeRegen -= 15;
				damage = 5;
			}

			if (afflicted)
			{
				drain = true;
				npc.lifeRegen -= 20;
				damage = 20;
			}

			if (iceCrush)
			{
				if (!npc.boss)
				{
					drain = true;
					float def = 2 + (npc.lifeMax / (npc.life * 1.5f));
					npc.lifeRegen -= (int)def;
					damage = (int)def;
				}
				else if (npc.boss || npc.type == NPCID.DungeonGuardian)
				{
					drain = true;
					npc.lifeRegen -= 6;
					damage = 3;
				}
			}

			if (death)
			{
				drain = true;
				npc.lifeRegen -= 10000;
				damage = 10000;
			}

			if (blaze)
			{
				drain = true;
				npc.lifeRegen -= 4;
				damage = 2;
			}

			if (noDamage)
				damage -= damageBefore;
			if (drain && before > 0)
				npc.lifeRegen -= before;
		}

		public override void GetChat(NPC npc, ref string chat)
		{
			Player player = Main.LocalPlayer;
			MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();

			if (Main.halloween && !Main.dayTime && AllowTrickOrTreat(npc) && modPlayer.CanTrickOrTreat(npc))
			{
				if (npc.type == NPCID.Guide && !player.HasItem(ModContent.ItemType<CandyBag>()))
				{
					chat = Language.GetTextValue("Mods.SpiritMod.TownNPC.Guide.Halloween");
					player.QuickSpawnItem(npc.GetSource_GiftOrReward(), ModContent.ItemType<CandyBag>());
				}
				else
				{
					chat = TrickOrTreat(modPlayer, npc);
					npc.DropItem(Main.rand.Next(ItemUtils.DropCandyTable()), npc.GetSource_GiftOrReward());
				}
			}
		}

		internal static bool AllowTrickOrTreat(NPC npc) => npc.type != NPCID.OldMan && npc.homeTileX != -1 && npc.homeTileY != -1 && !ModContent.GetInstance<SpiritMod>().NPCCandyBlacklist.Contains(npc.type);

		internal static string TrickOrTreat(MyPlayer player, NPC npc)
		{
			string name;
			int dialogue = Main.rand.Next(2);

			string modKey = null;
			if (npc.type == ModContent.NPCType<Adventurer>()) modKey = "Adventurer";
				else if (npc.type == ModContent.NPCType<Rogue>()) modKey = "Rogue";
				else if (npc.type == ModContent.NPCType<RuneWizard>()) modKey = "RuneWizard";
				else if (npc.type == ModContent.NPCType<Gambler>()) modKey = "Gambler";
    
			if (modKey != null)
				return Language.GetTextValue($"Mods.SpiritMod.TownNPC.{modKey}.Halloween.Dialogue{dialogue}");

			var vanillaKeys = new Dictionary<int, (string key, int maxDialogue)>()
			{
				{ NPCID.Merchant, ("Merchant", 3) },
				{ NPCID.Nurse, ("Nurse", 3) },
				{ NPCID.Dryad, ("Dryad", 3) },
				{ NPCID.Wizard, ("Wizard", 3) },
				{ NPCID.Mechanic, ("Mechanic", 3) },
				{ NPCID.Pirate, ("Pirate", 3) },
				{ NPCID.Angler, ("Angler", 3) },
				{ NPCID.TaxCollector, ("TaxCollector", 3) },
				{ NPCID.ArmsDealer, ("ArmsDealer", 2) },
				{ NPCID.Guide, ("Guide", 2) },
				{ NPCID.Demolitionist, ("Demolitionist", 2) },
				{ NPCID.Clothier, ("Clothier", 2) },
				{ NPCID.GoblinTinkerer, ("GoblinTinkerer", 2) },
				{ NPCID.SantaClaus, ("SantaClaus", 2) },
				{ NPCID.Truffle, ("Truffle", 2) },
				{ NPCID.Steampunker, ("Steampunker", 2) },
				{ NPCID.DyeTrader, ("DyeTrader", 2) },
				{ NPCID.PartyGirl, ("PartyGirl", 2) },
				{ NPCID.Cyborg, ("Cyborg", 2) },
				{ NPCID.Painter, ("Painter", 2) },
				{ NPCID.WitchDoctor, ("WitchDoctor", 2) },
				{ NPCID.Stylist, ("Stylist", 2) },
				{ NPCID.TravellingMerchant, ("TravellingMerchant", 2) },
				{ NPCID.SkeletonMerchant, ("SkeletonMerchant", 2) },
				{ NPCID.DD2Bartender, ("DD2Bartender", 2) }
			};

			if (vanillaKeys.TryGetValue(npc.type, out var info))
			{
				dialogue = Main.rand.Next(info.maxDialogue);
				string path = $"Mods.SpiritMod.TownNPC.Vanilla.{info.key}.Halloween.Dialogue{dialogue}";
				if (npc.type == NPCID.ArmsDealer && dialogue == 0 && player.Player.HeldItem.type == ItemID.CandyCornRifle)
					return Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.ArmsDealer.Halloween.DialogueSpecial");
				else if (npc.type == NPCID.Truffle && dialogue == 1 && (name = NPC.GetFirstNPCNameOrNull(NPCID.Nurse)) != null)
					return Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Truffle.Halloween.DialogueSpecial", name);
				else if (npc.type == NPCID.Painter && dialogue == 1)
					return Language.GetTextValue(path, player.Player.name);
				else if (npc.type == NPCID.WitchDoctor && dialogue == 1)
					return Language.GetTextValue(path, player.Player.name);
				else if (npc.type == NPCID.TravellingMerchant && dialogue == 0)
					return Language.GetTextValue(path, Main.worldName);
            
				return Language.GetTextValue(path);
			}

			return Language.GetTextValue($"Mods.SpiritMod.TownNPC.Default.Halloween.Dialogue{dialogue}", player.Player.name);
		}

		public override Color? GetAlpha(NPC npc, Color drawColor)
		{
			if (npc.HasBuff(ModContent.BuffType<TopazMarked>()))
				return Color.Lerp(base.GetAlpha(npc, drawColor) ?? Color.Transparent, new Color(158, 255, 253), 0.75f);
			return null;
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			bool surface = player.position.Y <= Main.worldSurface * 16 + NPC.sHeight;

			if (player.ZoneSpirit())
			{
				spawnRate = (int)(spawnRate * 0.73f);
				maxSpawns = (int)(maxSpawns * 1.1f);
			}

			if (player.ZoneAsteroid())
			{
				spawnRate = (int)(spawnRate * .7f);
				maxSpawns = (int)(maxSpawns * 1.1f);
			}

			if (MyWorld.blueMoon && surface)
			{
				spawnRate = (int)(spawnRate * 0.4f);
				maxSpawns = (int)(maxSpawns * 1.1f);
			}

			if (MyWorld.jellySky && (player.ZoneOverworldHeight || player.ZoneSkyHeight))
			{
				spawnRate = 2;
				maxSpawns = (int)(maxSpawns * 1.18f);
			}

			if (player.GetSpiritPlayer().oliveBranchBuff)
			{
				spawnRate = (int)(spawnRate * 4.5f);
				maxSpawns = (int)(maxSpawns * .5f);
			}

			if (player.HasBuff(ModContent.BuffType<LoomingPresence>()))
			{
				spawnRate = (int)(spawnRate * 0.8f);
				maxSpawns = (int)(maxSpawns * 1.1f);
			}
		}

		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;

			if (MyWorld.calmNight)
			{
				if (spawnInfo.Invasion || spawnInfo.Sky || MyWorld.blueMoon) 
					return; //if invasion or in sky

				if (Main.eclipse || Main.bloodMoon) 
					return; //if eclipse or blood moon

				if (!player.ZoneOverworldHeight) 
					return; //if not in overworld

				if (player.ZoneMeteor || player.ZoneRockLayerHeight || player.ZoneDungeon || player.ZoneBeach || player.ZoneCorrupt || player.ZoneCrimson || player.ZoneJungle || player.ZoneHallow || spawnInfo.Player.ZoneBriar() || spawnInfo.Player.ZoneSpirit()) 
					return; //if in wrong biome

				pool.Clear();
			}

			if (player.ZoneAsteroid())
			{
				pool.Clear();

				if (!spawnInfo.PlayerSafe)
				{
					pool.Add(ModContent.NPCType<Shockhopper.DeepspaceHopper>(), 0.30f);
					pool.Add(ModContent.NPCType<AstralAmalgam.AstralAmalgam>(), 0.16f);

					if (NPC.downedBoss2)
						pool.Add(ModContent.NPCType<Orbitite.Mineroid>(), 0.3f);

					pool.Add(ModContent.NPCType<Gloop.GloopGloop>(), 0.24f);

					if (NPC.downedBoss3)
						pool.Add(ModContent.NPCType<Starfarer.CogTrapperHead>(), 0.1f);
				}

				if ((NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3) && !NPC.AnyNPCs(ModContent.NPCType<MoonjellyEvent.DistressJelly>()))
					pool.Add(ModContent.NPCType<MoonjellyEvent.DistressJelly>(), .055f);
			}

			if (MyWorld.jellySky && (player.ZoneOverworldHeight || player.ZoneSkyHeight))
			{
				float multiplier = Main.pumpkinMoon || Main.bloodMoon || Main.snowMoon ? 0.05f : 1f;

				pool.Add(ModContent.NPCType<MoonjellyEvent.TinyLunazoa>(), 9.35f * multiplier);
				pool.Add(ModContent.NPCType<MoonjellyEvent.ExplodingMoonjelly>(), 8.35f * multiplier);
				pool.Add(ModContent.NPCType<MoonjellyEvent.MoonlightPreserver>(), 3.25f * multiplier);

				if (!NPC.AnyNPCs(ModContent.NPCType<MoonjellyEvent.MoonjellyGiant>()))
					pool.Add(ModContent.NPCType<MoonjellyEvent.MoonjellyGiant>(), .85f * multiplier);

				if (!NPC.AnyNPCs(ModContent.NPCType<MoonjellyEvent.DreamlightJelly>()))
					pool.Add(ModContent.NPCType<MoonjellyEvent.DreamlightJelly>(), .85f * multiplier);
			}

			if (MyWorld.blueMoon && (player.ZoneOverworldHeight || player.ZoneSkyHeight))
			{
				pool.Remove(0);
				pool.Add(ModContent.NPCType<MadHatter>(), 1f);
				pool.Add(ModContent.NPCType<LunarSlime>(), 3.4f);

				if (NPC.CountNPCS(ModContent.NPCType<Bloomshroom>()) < 2)
					pool.Add(ModContent.NPCType<Bloomshroom>(), 1f);

				if (NPC.CountNPCS(ModContent.NPCType<Glitterfly>()) < 3)
					pool.Add(ModContent.NPCType<Glitterfly>(), 1f);

				if (NPC.CountNPCS(ModContent.NPCType<GlowToad>()) < 4)
					pool.Add(ModContent.NPCType<GlowToad>(), .6f);

				if (NPC.CountNPCS(ModContent.NPCType<Lumantis>()) < 4)
					pool.Add(ModContent.NPCType<Lumantis>(), .6f);
			}

			if (player.active && player.ZoneLuminous())
			{
				pool.Clear();

				if (spawnInfo.Water)
				{
					if (LuminousWorld.LuminousType == LuminousWorld.GREEN)
						pool.Add(ModContent.NPCType<GreenAlgae2>(), 3f);
					else if (LuminousWorld.LuminousType == LuminousWorld.BLUE)
						pool.Add(ModContent.NPCType<BlueAlgae2>(), 3f);
					else if (LuminousWorld.LuminousType == LuminousWorld.PURPLE)
						pool.Add(ModContent.NPCType<PurpleAlgae2>(), 3f);
				}
			}
		}

		public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
		{
			if (npc.HasBuff(ModContent.BuffType<WitheringLeaf>()))
				modifiers.Defense.Flat -= 2;
			if (shadowbroken)
				modifiers.Defense.Flat -= 25;
			if (afflicted)
				modifiers.Defense.Flat -= 5;
		}

		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
		{
			bool summon = projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type] || projectile.sentry;

			if (summon)
				modifiers.FinalDamage.Flat += summonTag;
		}

		public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			bool summon = projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type] || projectile.sentry;

			if (sacrificialDaggerBuff && projectile.type != ModContent.ProjectileType<SacrificialDaggerProj>() && projectile.type != ModContent.ProjectileType<SacrificialDaggerProjectile>())
			{
				if (Main.rand.NextBool(4))
				{
					if (Main.netMode != NetmodeID.Server)
						SoundEngine.PlaySound(SoundID.Item71 with { PitchVariance = 0.2f, Volume = 0.5f }, npc.Center);

					int direction = npc.position.X > Main.player[projectile.owner].position.X ? 1 : -1;

					Vector2 randPos = npc.Center + new Vector2(direction, 0).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(70, 121);
					var dir = Vector2.Normalize(npc.Center - randPos) * 6;

					if (Main.netMode != NetmodeID.MultiplayerClient)
						Projectile.NewProjectile(projectile.GetSource_OnHit(npc), randPos.X, randPos.Y, dir.X, dir.Y, ModContent.ProjectileType<SacrificialDaggerProjectile>(), (int)(damageDone * 0.75f), 0, projectile.owner);

					DustHelper.DrawTriangle(npc.Center, 173, 5, 1.5f, 1f);
				}
			}
		}

		public override void OnKill(NPC npc)
		{
			Player closest = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];

			if (npc.townNPC && !GhostHandler.toBeGhosts.ContainsKey(npc.type))
				GhostHandler.toBeGhosts.Add(npc.type, npc.homeless ? Point.Zero : new Point(npc.homeTileX, npc.homeTileY));

			if (NPC.killCount[Item.NPCtoBanner(npc.BannerID())] == 50)
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/BannerSfx"), npc.Center);

			if (bloodInfused)
				Projectile.NewProjectile(npc.GetSource_Death(), npc.Center.X, npc.Center.Y, 0, 0, ModContent.ProjectileType<FlayedExplosion>(), 25, 0, Main.myPlayer);

			if (closest.GetSpiritPlayer().wayfarerSet)
				closest.AddBuff(ModContent.BuffType<Buffs.Armor.ExplorerFight>(), 240);

			bool lastTwin = (npc.type == NPCID.Retinazer && !NPC.AnyNPCs(NPCID.Spazmatism)) || (npc.type == NPCID.Spazmatism && !NPC.AnyNPCs(NPCID.Retinazer));
			if ((npc.type == NPCID.SkeletronPrime || npc.type == NPCID.TheDestroyer || lastTwin) && !MyWorld.spiritBiome)
				SpiritGeneration.SpawnSpiritBiome();
		}

		public override void OnSpawn(NPC npc, IEntitySource source)
		{
			if (npc.townNPC)
				GhostHandler.toBeGhosts.Remove(npc.type);
		}

		public override void DrawEffects(NPC npc, ref Color drawColor)
		{
			if (sFracture && Main.rand.NextBool(2))
				Dust.NewDust(npc.position, npc.width, npc.height, DustID.Firework_Yellow, (Main.rand.Next(8) - 4), (Main.rand.Next(8) - 4), 133);

			if (vineTrap)
				drawColor = new Color(103, 138, 84);
			if (clatterPierce)
				drawColor = new Color(115, 80, 57);
			if (tracked)
				drawColor = new Color(135, 245, 76);
			if (shadowbroken)
				drawColor = Color.Magenta;
		}
	}
}