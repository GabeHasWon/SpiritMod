using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;


namespace SpiritMod.NPCs.Starfarer
{
	public class CogTrapperBody : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stardancer");
			Main.npcFrameCount[NPC.type] = 1;
			NPCHelper.BuffImmune(Type, true);

			NPCID.Sets.SpawnFromLastEmptySlot[Type] = true;

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				Hide = true,
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.killCount[Type] = NPC.killCount[ModContent.NPCType<CogTrapperHead>()];
			NPC.damage = 25;
			NPC.npcSlots = 0f;
			NPC.width = 14;
			NPC.height = 20;
			NPC.defense = 14;
			NPC.lifeMax = 300;
			NPC.aiStyle = -1;
			AnimationType = 10;
			NPC.dontCountMe = true;
			NPC.knockBackResist = 0f;
			NPC.alpha = 255;
			NPC.behindTiles = true;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14;
			NPC.netAlways = true;
			NPC.dontCountMe = true;
			NPC.npcSlots = 0;

			AIType = -1;
			Banner = ModContent.NPCType<CogTrapperHead>();
			BannerItem = ModContent.ItemType<Items.Banners.StardancerBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.UIInfoProvider = new CustomEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<CogTrapperHead>()], false);
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

		public override void AI()
		{
			Player player = Main.player[NPC.target];
			Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0f, 0.0375f * 2, 0.125f * 2);

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.localAI[0] += Main.rand.Next(4);

				if (NPC.localAI[0] >= Main.rand.Next(700, 1000))
				{
					NPC.localAI[0] = 0f;
					NPC.TargetClosest(true);

					if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
					{
						SoundEngine.PlaySound(SoundID.Item9, NPC.Center);

						Vector2 targetPos = player.Center - NPC.Center + new Vector2(Main.rand.Next(-20, 21), Main.rand.Next(-20, 21));
						float speedMod = 1 / targetPos.Length();

						targetPos *= speedMod;
						targetPos += new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)) * 0.05f;

						int damage = Main.expertMode ? 10 : 15;
						int type = ModContent.ProjectileType<Starshock>();
						int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, targetPos.X, targetPos.Y, type, damage, 0f, Main.myPlayer, 0f, 0f);

						Main.projectile[proj].timeLeft = 180;
						NPC.netUpdate = true;
					}
				}
			}

			var parent = Main.npc[(int)NPC.ai[1]];

			if (!parent.active || (parent.type != ModContent.NPCType<CogTrapperHead>() && parent.type != Type))
			{
				NPC.life = 0;
				NPC.HitEffect(0, 10.0);
				NPC.active = false;
			}

			const int BodyLength = 12;

			if (parent.DistanceSQ(NPC.Center) > BodyLength * BodyLength)
				NPC.velocity = NPC.DirectionTo(parent.Center) * (parent.Distance(NPC.Center) - BodyLength);
			else
				NPC.velocity = Vector2.Zero;

			NPC.rotation = NPC.velocity.ToRotation() + 1.57f;

			if (parent.alpha < 128)
			{
				if (NPC.alpha != 0)
				{
					for (int i = 0; i < 2; i++)
					{
						int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 2f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].noLight = true;
					}
				}

				NPC.alpha -= 42;

				if (NPC.alpha < 0)
					NPC.alpha = 0;
			}
		}

		public override bool CheckActive() => false;
		public override bool PreKill() => false;

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, hitDirection, -1f, 0, default, 1f);

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Stardancer3").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Stardancer4").Type, 1f);

				NPC.position = NPC.Center;
				NPC.width = 20;
				NPC.height = 20;
				NPC.position.X = NPC.position.X - (NPC.width / 2f);
				NPC.position.Y = NPC.position.Y - (NPC.height / 2f);

				for (int i = 0; i < 5; i++)
				{
					int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, .5f);
					Main.dust[num622].velocity *= 2f;
				}

				for (int i = 0; i < 10; i++)
				{
					int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 1f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 4f;
					num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.DungeonSpirit, 0f, 0f, 100, default, .5f);
					Main.dust[num624].velocity *= 1f;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			var pos = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/Starfarer/CogTrapperBody_Glow").Value, screenPos);

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
			NPC.damage = (int)(NPC.damage * 0.65f);
		}
	}
}