using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.QuestSystem.Quests;
using SpiritMod.Items.Consumable.Quest;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.Hookbat
{
	public class Hookbat : ModNPC
	{
		public const int DASH_RATE = 300;
		public const int DASH_DELAY = 30;

		public ref float Counter => ref NPC.ai[0];

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 5;
			NPCID.Sets.TrailCacheLength[Type] = 3;
			NPCID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			NPC.width = 38;
			NPC.height = 38;
			NPC.damage = 10;
			NPC.rarity = 2;
			NPC.defense = 4;
			NPC.lifeMax = 42;
			NPC.knockBackResist = .53f;
			NPC.noGravity = true;
			NPC.value = 60f;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath4;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Surface NightTime");

		public override void AI()
		{
			NPC.TargetClosest(true);
			NPC.spriteDirection = NPC.direction;
			Player target = Main.player[NPC.target];

			if (!target.dead)
			{
				var targetPos = target.Center - new Vector2(0, 16 * 5);
				float speed = MathHelper.Clamp(NPC.Distance(targetPos) / 16, 0, 8);
				NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetPos) * speed, .0075f);

				if (Counter > DASH_RATE)
				{
					NPC.velocity *= .93f;
					if (Counter == (DASH_RATE + DASH_DELAY))
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							NPC.velocity = (NPC.DirectionTo(target.Center - target.velocity) * 17).RotatedByRandom(.13f);
							NPC.netUpdate = true;
						}
						SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, NPC.Center);
					}
					if (Counter > (DASH_RATE + DASH_DELAY + 20))
						Counter = 0;
				}
				if (Counter != DASH_RATE || (NPC.Distance(target.Center) < (16 * 8)))
					Counter++;
			}

			if (NPC.collideX)
				NPC.velocity.X = NPC.oldVelocity.X * -.5f;
			if (NPC.collideY)
				NPC.velocity.Y = NPC.oldVelocity.Y * -.5f;

			NPC.rotation = NPC.velocity.X * .1f;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (QuestManager.GetQuest<FirstAdventure>().IsActive && !spawnInfo.PlayerInTown)
				return 0.2f;

			return ((spawnInfo.SpawnTileY < Main.rockLayer && !spawnInfo.Player.ZoneOverworldHeight) || (!Main.dayTime && spawnInfo.Player.ZoneOverworldHeight)) && !spawnInfo.PlayerInTown && !NPC.AnyNPCs(ModContent.NPCType<Hookbat>()) ? 0.01f : 0;
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 10; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection * 2.5f, -1f, 0, default, Main.rand.NextFloat(.45f, 1.15f));

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				for (int i = 1; i < 4; ++i)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Hookbat" + i).Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Hookbat1").Type, 1f);
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => Counter > (DASH_RATE + DASH_DELAY);

		public override bool CanHitNPC(NPC target) => Counter > (DASH_RATE + DASH_DELAY);

		public override void OnKill()
		{
			if (QuestManager.GetQuest<FirstAdventure>().IsActive)
				Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ModContent.ItemType<DurasilkSheaf>());
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (Counter > (DASH_RATE + DASH_DELAY))
			{
				Vector2 drawOrigin = NPC.frame.Size() / 2;
				var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

				for (int k = 0; k < NPC.oldPos.Length; k++)
				{
					Vector2 drawPos = NPC.oldPos[k] - screenPos + (NPC.Size / 2) + new Vector2(0f, NPC.gfxOffY);
					Color color = NPC.GetAlpha(drawColor) * (float)(((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
			return true;
		}

		public override void FindFrame(int frameHeight)
		{
			float rate = 1f / (6 + (int)(MathHelper.Max(Counter - DASH_RATE, 0) * .2f));
			NPC.frameCounter = (NPC.frameCounter + rate) % 4;
			NPC.frame.Y = frameHeight * (int)NPC.frameCounter;
		}
	}
}