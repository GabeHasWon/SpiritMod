using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.IO;
using SpiritMod.Buffs;
using SpiritMod.Mechanics.BoonSystem;
using SpiritMod.Buffs.DoT;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.StymphalianBat
{
	public class StymphalianBat : ModNPC, IBoonable
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stymphalian Bat");
			Main.npcFrameCount[NPC.type] = 7;
			NPCID.Sets.TrailCacheLength[NPC.type] = 2;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
			NPCHelper.ImmuneTo<FesteringWounds, BloodCorrupt, BloodInfusion>(this, BuffID.Poisoned, BuffID.Venom);
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 40;
			NPC.damage = 50;
			NPC.defense = 21;
			NPC.lifeMax = 155;
			NPC.knockBackResist = .23f;
			NPC.noGravity = true;
			NPC.value = 560f;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath4;

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.StymphalianBatBanner>();
		}

		private int Counter
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private bool Diving
		{
			get => NPC.ai[1] > 0;
			set => NPC.ai[1] = value ? 1 : 0;
		}
		private bool Collided
		{
			get => NPC.ai[2] > 0;
			set => NPC.ai[2] = value ? 1 : 0;
		}

		private bool JustSpawned
		{
			get => NPC.ai[3] > 0;
			set => NPC.ai[3] = value ? 1 : 0;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
			{
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Marble,
				new FlavorTextBestiaryInfoElement("Ancient blueprints. Recent fabrication. These designs were perfected long ago, there is no need to deviate from his design."),
			});
		}

		private int frame;

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			if (Main.rand.NextBool(3))
				target.AddBuff(BuffID.Bleeding, 3600);
		}

		public override void AI()
		{
			if (!JustSpawned)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int npc1 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-100, 100), (int)NPC.Center.Y + Main.rand.Next(-100, 100), ModContent.NPCType<StymphalianBat>(), NPC.whoAmI, 40f, 0f, 0f, 1f);
					NPC.netUpdate = true;
					Main.npc[npc1].netUpdate = true;

					JustSpawned = true;

					for (int i = 0; i < 3; ++i)
						Gore.NewGore(NPC.GetSource_Death(), Main.npc[npc1].position, Main.rand.NextVector2Circular(4f, 4f), 99);
				}
			}

			NPC.spriteDirection = NPC.direction;
			Player target = Main.player[NPC.target];

			Counter++;
			if (!target.dead && !Diving)
			{
				if (NPC.collideX)
				{
					NPC.velocity.X = NPC.oldVelocity.X * -0.5f;
					if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
						NPC.velocity.X = 2f;
					if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
						NPC.velocity.X = -2f;
				}
				if (NPC.collideY)
				{
					NPC.velocity.Y = NPC.oldVelocity.Y * -0.5f;
					if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1f)
						NPC.velocity.Y = 1f;
					if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1f)
						NPC.velocity.Y = -1f;
				}

				NPC.TargetClosest(true);

				if (NPC.direction == -1 && NPC.velocity.X > -7f)
				{
					NPC.velocity.X = NPC.velocity.X - 0.26f;
					if (NPC.velocity.X > 7f)
						NPC.velocity.X = NPC.velocity.X - 0.26f;
					else if (NPC.velocity.X > 0f)
						NPC.velocity.X = NPC.velocity.X - 0.05f;
					if (NPC.velocity.X < -7f)
						NPC.velocity.X = -7f;
				}
				else if (NPC.direction == 1 && NPC.velocity.X < 7f)
				{
					NPC.velocity.X = NPC.velocity.X + 0.26f;
					if (NPC.velocity.X < -7f)
						NPC.velocity.X = NPC.velocity.X + 0.26f;
					else if (NPC.velocity.X < 0f)
						NPC.velocity.X = NPC.velocity.X + 0.05f;
					if (NPC.velocity.X > 7f)
						NPC.velocity.X = 7f;
				}

				float num3225 = Math.Abs(NPC.Center.X - target.Center.X);
				float num3224 = target.position.Y - (NPC.height / 2f);

				if (num3225 > 50f)
					num3224 -= 150f;
				if (NPC.position.Y < num3224)
				{
					NPC.velocity.Y = NPC.velocity.Y + 0.05f;
					if (NPC.velocity.Y < 0f)
						NPC.velocity.Y = NPC.velocity.Y + 0.01f;
				}
				else
				{
					NPC.velocity.Y = NPC.velocity.Y - 0.05f;
					if (NPC.velocity.Y > 0f)
						NPC.velocity.Y = NPC.velocity.Y - 0.01f;
				}
				if (NPC.velocity.Y < -4f)
					NPC.velocity.Y = -4f;
				if (NPC.velocity.Y > 4f)
					NPC.velocity.Y = 3f;
			}

			if (Diving)
			{
				if (NPC.collideX || NPC.collideY)
				{
					if (!Collided)
					{
						SoundEngine.PlaySound(SoundID.Tink, NPC.position);
						Collision.TileCollision(NPC.position, NPC.velocity, NPC.width, NPC.height);

						Collided = true;
					}
				}

				if (Collided) //Stuck in a tile
				{
					if (Main.netMode != NetmodeID.Server)
					{
						NPC.rotation += Main.rand.NextFloat(-0.06f, 0.06f);
						DrawOffsetY = 15;
					}

					NPC.velocity = Vector2.Zero;
					NPC.netUpdate = true;
				}
				else if (NPC.velocity != Vector2.Zero)
				{
					NPC.rotation = NPC.velocity.ToRotation() + ((NPC.direction == -1) ? MathHelper.Pi : 0);
				}
			}
			else
			{
				NPC.rotation = NPC.velocity.X * .1f;
			}

			if (Counter == 205)
			{
				if (!Diving)
				{
					SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, NPC.Center);

					Vector2 direction = NPC.DirectionTo(target.Center);
					direction.X *= Main.rand.Next(16, 22);
					direction.Y *= Main.rand.Next(10, 15);

					NPC.velocity.X = direction.X;
					NPC.velocity.Y = direction.Y;

					Diving = true;
				}

				NPC.netUpdate = true;
			}

			if (Counter > 265)
			{
				Counter = 0;
				Diving = false;
				Collided = false;

				NPC.netUpdate = true;

				DrawOffsetY = 0;
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => (spawnInfo.SpawnTileType == 367) && spawnInfo.Player.ZoneMarble && spawnInfo.SpawnTileY > Main.rockLayer && Main.hardMode ? 0.435f : 0f;

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 10; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection * 2.5f, -1f, 0, default, Main.rand.NextFloat(.45f, 1.15f));
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith, 2.5f * hitDirection, -2.5f, 0, default, 0.27f);
			}
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				for (int i = 0; i < 3; ++i)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 99);
				for (int i = 1; i < 5; ++i)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("StymphalianBat" + i).Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("StymphalianBat1").Type, 1f);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon(ItemID.AdhesiveBandage, 100);
			npcLoot.AddCommon<Items.Accessory.GoldenApple>(85);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (Diving && !Collided)
			{
				Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width * 0.5f, NPC.height * 0.5f);
				for (int k = 0; k < NPC.oldPos.Length; k++)
				{
					var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
					Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);
					Color color = NPC.GetAlpha(drawColor) * (float)((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length / 2f);
					spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
			return true;
		}

		public override void FindFrame(int frameHeight)
		{
			if (++NPC.frameCounter >= 6)
			{
				frame++;
				NPC.frameCounter = 0;
			}

			if (Diving)
				frame = Main.npcFrameCount[Type] - 1;
			else if (frame > (Main.npcFrameCount[Type] - 2))
				frame = 0;

			NPC.frame.Y = frameHeight * frame;
		}
	}
}