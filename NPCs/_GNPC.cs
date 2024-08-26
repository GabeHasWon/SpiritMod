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

		public bool soulBurn = false;
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
			soulBurn = false;
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

			if (soulBurn)
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
					chat = "Take this bag; you can use it to store your Candy. \"How do I get candy?\", you ask? Try talking to the other villagers.";
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
			switch (npc.type)
			{
				case NPCID.Merchant:
					if (dialogue == 0)
						return "Oh, here's some candy. You have no idea how hard it was to get this.";
					else if (dialogue == 1)
						return "I can be greedy, but I like to be festive. Here you go!";
					else
						return "I'll give you this piece for free. I can't give you any promises about the next piece.";
				case NPCID.Nurse:
					if (dialogue == 0)
						return "Here, kiddo. Make sure there aren't any razorblades in there!";
					else if (dialogue == 1)
						return "It may not be the healithiest option, but candy is pretty nice. Take some.";
					else
						return "I'm pretty sure this candy that makes you healthier. Maybe. Don't quote me on this.";
				case NPCID.ArmsDealer:
					if (dialogue == 0)
						if (player.Player.HeldItem.type == ItemID.CandyCornRifle)
							return "Is that... it is! A Candy Corn Rifle! Here, I want you to have this for showing it to me.";
						else
							return "I hear there is a gun that shoots candy. Oh what I wouldn't give for one. What? Oh, yes, here's your candy.";
					else
						return "You wouldn't believe it. I asked for ammo, but my suppliers gave me candy instead! You want a piece?";
				case NPCID.Dryad:
					if (dialogue == 0)
						return "Do you have any idea what's in that candy? Here, this stuff is much better for you. I made it myself.";
					else if (dialogue == 1)
						return "Is it that time of year again? Time flies by so fast when you are as old as I am... Oh, here, have some candy.";
					else
						return "I wish I had candy seeds to sell you. Growing sweets would be far more sustainable than going door-to-door asking for them.";
				case (NPCID.Guide):
					if (dialogue == 0)
						return "Here. You may collect one piece of candy a night from every villager during halloween.";
					else
						return "Candy can be used during any season to get special effects.";
				case NPCID.Demolitionist:
					if (dialogue == 0)
						return "I was making a sugar rocket, and this was left over. Do you want some?";
					else
						return "Ach, this candy may or may not have explosives in it, I don't remember.";
				case NPCID.Clothier:
					if (dialogue == 0)
						return "My mama always told me candy was like life. Or was it a box? ... er, something like that. Here, take a piece.";
					else
						return "I'm quite the candy enthusiast. You want a piece?";
				case NPCID.GoblinTinkerer:
					if (dialogue == 0)
						return "I tried combining rocket boots and candy, but it didn't really work out. You want what's left?";
					else
						return "It turns out putting together every flavor of candy tastes pretty bad. I know, I'm disappointed too.";
				case NPCID.Wizard:
					if (dialogue == 0)
						return "I'm pretty sure this isn't enchanted candy, but I could make some if you want! No? Ok...";
					else if (dialogue == 1)
						return "I have some candy for you, but I could enchant it if you would li... No? Ok.";
					else
						return "Are you sure you don't want enchanted candy? It wouldn't be a bother if I just... No? Fine...";
				case NPCID.Mechanic:
					if (dialogue == 0)
						return "Don't mind the hydraulic fluid on the candy. In fact, consider it extra flavor.";
					else if (dialogue == 1)
						return "If you keep this candy in your pocket, it can monitor your heart rate, blood pressure, and tell how many steps you take!";
					else
						return "It turns out you can't make an engine powered by candy. Birds are fine, but candy? Too much, apparently.";
				case NPCID.SantaClaus:
					if (dialogue == 0)
						return "Something isn't right. This feels all wrong.";
					else
						return "Ho ho ho! 'Tis the season -- wait, 'tisn't the season! What am I doing here?";
				case NPCID.Truffle:
					if (dialogue == 0)
						return "Is this candy vegan? Of course not, you sicko!";
					else if ((name = NPC.GetFirstNPCNameOrNull(NPCID.Nurse)) != null)
						return name + " wanted some of these for her supply. I wonder what that was about?";
					else
						return "What do you mean there's mold on this piece? That's clearly fungus!";
				case NPCID.Steampunker:
					if (dialogue == 0)
						return "All hallow's eve, you say? Cor, I've got just the thing! Here, have some treacle!";
					else
						return "I suppose you want some puddings, yeah? Here you are, love!";
				case NPCID.DyeTrader:
					if (dialogue == 0)
						return "I put some special dyes in this sweet. It will make your tongue turn brilliant colors!";
					else
						return "It isn't about how it tastes... It's about how rich the colors look. Take a piece, why don't you?";
				case NPCID.PartyGirl:
					if (dialogue == 0)
						return "I love this time of year! Now I don't need an excuse give out free candy! Here, have a piece!";
					else
						return "Who ever said you needed drugs to party? Candy is waaaay better!";
				case NPCID.Cyborg:
					if (dialogue == 0)
						return "My calendar programming has determined that it is approximately Halloween; enjoy your sucrose based food!";
					else
						return "Sugar always seems to gum up my inaards. Here, hold this candy while I try and fix that.";
				case NPCID.Painter:
					if (dialogue == 0)
						return "I might've dripped a little paint on this candy, but it's probably lead-free. Hopefully.";
					else
						return "Oh, " + player.Player.name + ", you want candy? Let me get your portrait, then you can have some.";
				case NPCID.WitchDoctor:
					if (dialogue == 0)
						return "I decided not to give you lemon heads... or, should I say, lemon-flavored heads. Enjoy!";
					else
						return "Beware, " + player.Player.name + ", for it is the season of ghouls and spirits. This edible talisman will protect you.";
				case NPCID.Pirate:
					if (dialogue == 0)
						return "Yo ho ho and a bottle of... candy. Take some!";
					else if (dialogue == 1)
						return "This candy cost me an arm and a leg. Enjoy that now, or it's to the plank with ye!";
					else
						return "Arrr, there's an old sayin' that goes \"Do what ye want, 'cause a pirate is free.\" I'd like to think that applies to eatin' candy as well!";
				case NPCID.Stylist:
					if (dialogue == 0)
						return "I usually save these for after haircuts, but go ahead and take a piece, darling.";
					else
						return "I've got plenty of candy, hon! Take as much as you want.";
				case NPCID.TravellingMerchant:
					if (dialogue == 0)
						return "I have rare candies from all over " + Main.worldName + ". Here, take some.";
					else
						return "I hear in far-off lands they have candy so sour it will melt your tongue! Unfortunately, I only have mundane candy for you.";
				case NPCID.Angler:
					if ((dialogue = Main.rand.Next(3)) == 0)
						return "What? You want some of MY candy? I think I have some ichorice here somewhere...";
					else if (dialogue == 1)
						return "This one came out of a fish. Here, you have it, I don't want it";
					else
						return "Dude, you don't ask a kid for candy. You just don't.";
				case NPCID.TaxCollector:
					if ((dialogue = Main.rand.Next(3)) == 0)
						return "Halloween? Bah, humbug! Take your candy and get out.";
					else if (dialogue == 1)
						return "You come to my door to take my sweets? Well go on then, take 'em!";
					else
						return "Here, have this. It's the cheapest brand I could find.";
				case NPCID.SkeletonMerchant:
					if (dialogue == 0)
						return "I'm feeling happy, it's my people's season! Take some candy!";
					else
						return "Did you know that rock candy doesn't actually grow underground? Oh, you did? Hmph.";
				case NPCID.DD2Bartender:
					if (dialogue == 0)
						return "I managed to find some ale-flavored candy! Maybe this world ain't so bad after all.";
					else
						return "Wiping a counter all day has made me appreciate the little things in life, like candy. Care for a piece?";
			}

			if (npc.type == ModContent.NPCType<Adventurer>())
			{
				if (dialogue == 0)
					return "You wouldn't believe me if I told you I got this from a faraway kingdom made of CANDY! I promise it has an exquisite taste.";
				else
					return "I hear you can get more candy from the goodie bags that monsters hold. As if I needed an excuse to slay some zombies!";
			}
			else if (npc.type == ModContent.NPCType<Rogue>())
			{
				if (dialogue == 0)
					return "You want some candy? Here, catch!";
				else
					return "Hiyah! Candy attack! Oh, it's you. sorry.";
			}
			else if (npc.type == ModContent.NPCType<RuneWizard>())
			{
				if (dialogue == 0)
					return "Behold! Enchanted candy! Enchantingly tasty, that is!";
				else
					return "Watch closely, for I shall channel the power of the spirits to summon... Candy!";

			}
			else if (npc.type == ModContent.NPCType<Gambler>())
			{
				if (dialogue == 0)
					return "Reach into the bowl. You never know what you'll pull out";
				else
					return "I'll trade you any piece of candy for a random pie- no? Ok";
			}

			if (dialogue == 0)
				return "Hello, " + player.Player.name + ". Take some candy!";
			else
				return "Here, I have some candy for you.";
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

			if (player.active && player.ZoneBeach && MyWorld.luminousOcean && !Main.dayTime)
			{
				pool.Clear();

				if (spawnInfo.Water)
				{
					if (MyWorld.luminousType == 1)
						pool.Add(ModContent.NPCType<GreenAlgae2>(), 3f);
					else if (MyWorld.luminousType == 2)
						pool.Add(ModContent.NPCType<BlueAlgae2>(), 3f);
					else if (MyWorld.luminousType == 3)
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