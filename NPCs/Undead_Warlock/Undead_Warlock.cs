using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using System.Linq;

namespace SpiritMod.NPCs.Undead_Warlock
{
	public class Undead_Warlock : ModNPC
	{
		private bool SpawningCrystals => spawningCrystalsTimer >= 240;

		private int spawningCrystalsTimer = 0;
		private int crystalTimer = 0;
		private int projectileTimer = 0;
		private int resetTimer = 0;

		private int spawnedProjectiles = 0;

		private int[] crystals = new int[3];

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Undead Warlock");
			Main.npcFrameCount[NPC.type] = 5;
		}

		public override void SetDefaults()
		{
			NPC.aiStyle = 3;
			NPC.lifeMax = 150;
			NPC.defense = 7;
			NPC.value = 100f;
			AIType = 3;
			NPC.knockBackResist = 0.2f;
			NPC.width = 24;
			NPC.height = 36;
			NPC.damage = 28;
			NPC.lavaImmune = false;
			NPC.HitSound = SoundID.NPCHit1;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Unlike the Undead Scientist, the Warlock practices more typical, but more powerful, magic and necromancy."),
			});
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(spawningCrystalsTimer);
			writer.Write(crystalTimer);
			writer.Write(spawnedProjectiles);
			writer.Write(projectileTimer);
			writer.Write(resetTimer);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			spawningCrystalsTimer = reader.ReadInt32();
			crystalTimer = reader.ReadInt32();
			spawnedProjectiles = reader.ReadInt32();
			projectileTimer = reader.ReadInt32();
			resetTimer = reader.ReadInt32();
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
			if ((NPC.velocity.X != 0f && NPC.velocity.Y == 0f) || NPC.IsABestiaryIconDummy)
			{
				if (NPC.frameCounter >= 7)
				{
					NPC.frameCounter = 0;
					NPC.frame.Y = (NPC.frame.Y + 1) % 5 * frameHeight;
				}
			}
			else
				NPC.frame.Y = 2 * frameHeight;
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
			NPC.spriteDirection = NPC.direction;

			spawningCrystalsTimer++;

			if (SpawningCrystals)
			{
				NPC.aiStyle = -1;
				NPC.velocity.X = 0f;

				if (NPC.collideY)
				{
					crystalTimer++;

					if (crystalTimer > 45)
						projectileTimer++;

					if (crystalTimer == 45 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

						for (int i = 0; i < 3; ++i)
						{
							crystals[i] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y - 90, 0f, 0f, ModContent.ProjectileType<Undead_Warlock_Crystal>(), 10, 3f, 0);
							Main.projectile[crystals[i]].Center += new Vector2(0, 40).RotatedBy(MathHelper.ToRadians(120 * (i + 1)));
							Main.projectile[crystals[i]].ai[1] = NPC.whoAmI;
						}
					}

					if (projectileTimer % 60 == 1)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							int[] types = new int[] { ModContent.ProjectileType<Letter_1>(), ModContent.ProjectileType<Letter_2>(), ModContent.ProjectileType<Letter_3>(), ModContent.ProjectileType<Letter_4>() };
							int chosenProjectile = Main.rand.Next(types);

							spawnedProjectiles++;
							SoundEngine.PlaySound(SoundID.Item28, NPC.Center);

							int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 2, NPC.Center.Y - 88, 0f, 0f, chosenProjectile, 20, 3f, 0);
							Main.projectile[p].ai[1] = NPC.whoAmI;
						}

						int chosenDust = Main.rand.NextBool(2) ? 173 : 157;

						for (int i = 0; i < 10; ++i)
						{
							int index2 = Dust.NewDust(new Vector2(NPC.Center.X + 2, NPC.Center.Y - 88), 0, 0, chosenDust, 0.0f, 0.0f, 0, new Color(), 0.75f);
							Main.dust[index2].velocity *= 1.2f;
							--Main.dust[index2].velocity.Y;
							Main.dust[index2].position = Vector2.Lerp(Main.dust[index2].position, new Vector2(NPC.Center.X + 2, NPC.Center.Y - 88), 0.75f);
						}
					}

					if (spawnedProjectiles >= 2)
						resetTimer++;

					if (resetTimer > 200)
					{
						for (int i = 0; i < 3; ++i)
							Main.projectile[crystals[i]].Kill();

						crystalTimer = 0;
						spawningCrystalsTimer = 0;
						spawnedProjectiles = 0;
						projectileTimer = 0;
						resetTimer = 0;
						NPC.netUpdate = true;
					}

					if (crystals.All(x => Main.projectile[x].active) && crystalTimer > 59)
					{
						DrawDustBeetweenThisAndThat(Main.projectile[crystals[0]].Center, Main.projectile[crystals[1]].Center);
						DrawDustBeetweenThisAndThat(Main.projectile[crystals[1]].Center, Main.projectile[crystals[2]].Center);
						DrawDustBeetweenThisAndThat(Main.projectile[crystals[2]].Center, Main.projectile[crystals[0]].Center);

						if (Main.rand.NextBool(2))
						{
							for (int i = 0; i < 7; i++)
							{
								int chosenDust = Main.rand.NextBool(2) ? 173 : 157;
								Vector2 offset = new Vector2();
								double angle = Main.rand.NextDouble() * 2d * Math.PI;
								offset.X += (float)(Math.Sin(angle) * 17f);
								offset.Y += (float)(Math.Cos(angle) * 17f);
								Dust dust = Main.dust[Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y - 88) + offset - new Vector2(2, 4), 0, 0, chosenDust, 0, 0, 0, new Color(), 0.95f)];
								dust.velocity = Vector2.Zero;
								dust.fadeIn = 0.2f;
								dust.noGravity = true;
								dust.noLight = false;
							}
						}
					}
				}
			}
			else
			{
				if (NPC.aiStyle != 3)
					NPC.aiStyle = 3;

				NPC.netUpdate = true;
			}
		}

		public static void DrawDustBeetweenThisAndThat(Vector2 vector3, Vector2 vector1)
		{
			Vector2 range = vector3 - vector1;
			if (Main.rand.NextBool(12))
			{
				for (int i = 0; i < 8; i++)
				{
					int chosenDust = Main.rand.NextBool(2) ? 173 : 157;
					Dust dust = Main.dust[Dust.NewDust(vector1 + range * Main.rand.NextFloat() + Vector2.Zero, 0, 0, chosenDust)];
					dust.noGravity = true;
					dust.noLight = false;
					dust.velocity = range * 0.001f;
					dust.scale = 1.24f;
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon(ItemID.Shackle, 3);
			npcLoot.AddCommon(ItemID.ZombieArm, 10);
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				SoundEngine.PlaySound(SoundID.Item9, NPC.Center);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("UndeadWarlock_0").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("UndeadWarlock_2").Type, 1f);

				for (int i = 0; i < 2; ++i)
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("UndeadWarlock_1").Type, 1f);
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("UndeadWarlock_3").Type, 1f);
				}

				NPC.netUpdate = true;
			}

			for (int k = 0; k < 7; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.7f);
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (NPC.AnyNPCs(ModContent.NPCType<Undead_Warlock>()) || spawnInfo.PlayerSafe)
				return 0f;
			return SpawnCondition.OverworldNightMonster.Chance * 0.001f;
		}
	}
}