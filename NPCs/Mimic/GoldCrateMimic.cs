using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Mimic
{
	public class GoldCrateMimic : ModNPC
	{
		bool jump = false;

		private bool Defending => NPC.DistanceSQ(Main.player[NPC.target].Center) > 720 * 720;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;
			NPCID.Sets.TrailCacheLength[NPC.type] = 3;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
		}

		public override void SetDefaults()
		{
			NPC.width = 46;
			NPC.height = 40;
			NPC.damage = 22;
			NPC.defense = 12;
			NPC.lifeMax = 110;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 460f;
			NPC.knockBackResist = .15f;
			NPC.aiStyle = 41;
			AIType = NPCID.Herpling;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.GoldCrateMimicBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.AddInfo(this, "Ocean");
			bestiaryEntry.UIInfoProvider = new CustomEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], false, 10);
		}

		int frame = 2;

		public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

			if (Defending)
			{
				NPC.TargetClosest();
				NPC.spriteDirection = 1;
				NPC.velocity = Vector2.UnitY * 7f;
				NPC.defense = 100;
			}
			else
			{
				NPC.defense = NPC.defDefense;

				if (NPC.collideY && jump && NPC.velocity.Y > 0)
					if (Main.rand.NextBool(2))
					{
						jump = false;
						for (int i = 0; i < 20; i++)
						{
							int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.SpookyWood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
							Main.dust[dust].noGravity = true;
						}
					}
				if (!NPC.collideY)
					jump = true;
			}
		}

		public override void FindFrame(int frameHeight)
		{
			if (!Defending || NPC.IsABestiaryIconDummy)
			{
				if (++NPC.frameCounter == 5)
				{
					frame++;
					NPC.frameCounter = 0;
				}
				if (frame >= 4)
					frame = 1;
			}
			else frame = 0;

			NPC.frame.Y = frameHeight * frame;
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			if (Defending && !projectile.minion && !projectile.sentry && !Main.player[projectile.owner].channel)
			{
				projectile.hostile = true;
				projectile.friendly = false;
				projectile.penetrate = 2;
				projectile.velocity.X *= -1f;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (!Defending)
			{
				Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width * 0.5f, NPC.height / Main.npcFrameCount[NPC.type] * 0.5f);
				for (int k = 0; k < NPC.oldPos.Length; k++)
				{
					var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
					Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);
					Color color = NPC.GetAlpha(drawColor) * (float)(((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
			return true;
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity / 6, 220);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity / 6, 221);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity / 6, 222);
			}
			for (int k = 0; k < 6; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sunflower, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.47f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sunflower, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, .57f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sunflower, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, .77f);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ItemID.GoldenCrate));
	}
}