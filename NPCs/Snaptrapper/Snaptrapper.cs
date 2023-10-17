using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using SpiritMod.Items.Sets.ClubSubclass;
using SpiritMod.Items.Weapon.Summon;
using SpiritMod.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;

namespace SpiritMod.NPCs.Snaptrapper
{
    public class Snaptrapper : ModNPC, IBCRegistrable
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
			NPCHelper.ImmuneTo<Buffs.DoT.FesteringWounds>(this, BuffID.Poisoned, BuffID.Venom);

			var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Position = new Vector2(-10, 10),
				PortraitPositionYOverride = 0,
				Velocity = 1f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 62;
			NPC.height = 74;
			NPC.damage = 25;
			NPC.defense = 12;
			NPC.lifeMax = 340;
    		NPC.rarity = 3;
			NPC.HitSound = SoundID.NPCHit32;
            NPC.DeathSound = SoundID.NPCDeath25;
			NPC.value = Item.buyPrice(0, 0, 50, 0);
			NPC.knockBackResist = 0f;
			NPC.aiStyle = 3;

			AIType = NPCID.SnowFlinx;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.AntlionAssassinBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Jungle");

		public override bool PreKill()
        {
            MyWorld.downedSnaptrapper = true;
            return true;
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddOneFromOptions<SnapsporeStaff, SporeClub>(2);

		public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 6; i++)
            {
                int a = Gore.NewGore(NPC.GetSource_OnHit(NPC), new Vector2(NPC.Center.X + Main.rand.Next(-10, 10), NPC.Center.Y + Main.rand.Next(-10, 10)), NPC.velocity, 911);
                Main.gore[a].timeLeft = 20;
                Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
            }

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
            {
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/DownedMiniboss"), NPC.Center);

				for (int j = 0; j < 12; j++)
                {
                    int a = Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.Center.X + Main.rand.Next(-10, 10), NPC.Center.Y + Main.rand.Next(-10, 10)), NPC.velocity, 911);
                    Main.gore[a].timeLeft = 20;
                    Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                }

                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Snaptrapper1").Type, 1f);

                for (int k = 0; k < 6; k++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Snaptrapper2").Type, Main.rand.NextFloat(.6f, 1f));
                for (int z = 0; z < 2; z++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Snaptrapper3").Type, Main.rand.NextFloat(.8f, 1f));
            }
        }

		float frameCounter;

		public override void FindFrame(int frameHeight)
		{
			int frameWidth = 88;
			int frameX;

			if (chargePhase) //Charge
			{
				frameX = 1;
			}
			else if (!chargePhase && melee) //Attack at close range
			{
				frameX = 1;
			}
			else //Walk
			{
				frameX = 0;
			}

			frameCounter = (frameCounter += .15f) % Main.npcFrameCount[Type];
			int frameY = (int)frameCounter;

			NPC.frame = new Rectangle(frameX * frameWidth, frameY * frameHeight, frameWidth - 2, frameHeight - 2);
		}

		bool melee;
		bool chargePhase;

		public override void AI()
		{
            NPC.spriteDirection = NPC.direction;
            Player target = Main.player[NPC.target];
            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * new Vector2(3.75f, 8f);
			float distance = NPC.DistanceSQ(target.Center);

			if (melee)
			{
				NPC.velocity.X *= .9f;
				NPC.noGravity = false;
				NPC.spriteDirection = target.position.X < NPC.position.X ? -1 : 1;

				if ((frameCounter + .15f) >= (Main.npcFrameCount[Type] - 1))
					melee = false;
			}
			if (!melee && !chargePhase && distance < (62 * 62))
            {
				frameCounter = 0;
				melee = true;
			}

			NPC.localAI[1]++;
            if (NPC.life >= NPC.lifeMax * .4f)
            {
                if (NPC.localAI[1] >= 340 && NPC.localAI[1] < 400)
                {
                    NPC.velocity.X = 0f;
                    NPC.noGravity = false;
					NPC.spriteDirection = target.position.X < NPC.position.X ? -1 : 1;

					if (Main.rand.NextBool(3))
                    {
                        Dust dust = Main.dust[Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + 2f), NPC.width, NPC.height, ModContent.DustType<Dusts.PoisonGas>(), NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, new Color(), 7f)];
                        dust.noGravity = true;
                        dust.velocity.X *= 0.3f;
                        dust.velocity.Y = (dust.velocity.Y * 0.2f) - 1;
                    }
                }

                if (NPC.localAI[1] == 360)
                {
                    SoundEngine.PlaySound(SoundID.Grass, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int amountOfProjectiles = Main.rand.Next(2, 4);
                        for (int i = 0; i < amountOfProjectiles; ++i)
                        {
                            float A = Main.rand.Next(-60, 60) * 0.05f;
                            float B = Main.rand.Next(-80, -10) * 0.05f;
                            int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, direction.X + A, -direction.Y + B, ModContent.ProjectileType<SnaptrapperSpore>(), 18, 1, Main.myPlayer, 0, 0);
                            Main.projectile[p].hostile = true;
                            Main.projectile[p].friendly = false;
                        }
                    }
                }

                if (NPC.localAI[1] == 400)
                    SoundEngine.PlaySound(SoundID.NPCDeath5, NPC.Center);

                if (NPC.localAI[1] >= 400 && NPC.localAI[1] <= 530)
                {
                    NPC.netUpdate = true;

                    Charging();

                    if (NPC.collideY)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + 2f), NPC.width, NPC.height, ModContent.DustType<Dusts.PoisonGas>(), NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, new Color(), 5.3f)];
                            dust.noGravity = true;
                            dust.velocity.X *= 0.3f;
                            dust.velocity.Y = (dust.velocity.Y * 0.2f) - 1;
                        }
                        frameCounter = 1;
                    }
					else
						frameCounter = 2;
                }
                else
                {
                    chargePhase = false;
                    NPC.aiStyle = 3;
                    AIType = NPCID.SnowFlinx;
                }

                if (NPC.localAI[1] >= 800)
                    NPC.localAI[1] = 0;
            }
			else
            {
                if (NPC.localAI[1] >= 240 && NPC.localAI[1] < 300)
                {
                    NPC.velocity.X = 0f;
                    NPC.noGravity = false;
					NPC.spriteDirection = target.position.X < NPC.position.X ? -1 : 1;

					if (Main.rand.NextBool(2))
                    {
                        Dust dust = Main.dust[Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + 2f), NPC.width, NPC.height, ModContent.DustType<Dusts.PoisonGas>(), NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, new Color(), 7f)];
                        dust.noGravity = true;
                        dust.velocity.X *= 0.3f;
                        dust.velocity.Y = (dust.velocity.Y * 0.2f) - 1;

                    }
                }

                if (NPC.localAI[1] == 260)
                {
                    SoundEngine.PlaySound(SoundID.Grass, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int amountOfProjectiles = Main.rand.Next(3, 6);
                        for (int i = 0; i < amountOfProjectiles; ++i)
                        {
                            float A = Main.rand.Next(-60, 60) * 0.05f;
                            float B = Main.rand.Next(-80, -10) * 0.05f;
                            int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, direction.X + A, -direction.Y + B, ModContent.ProjectileType<SnaptrapperSpore>(), 18, 1, Main.myPlayer, 0, 0);
                            Main.projectile[p].hostile = true;
                            Main.projectile[p].friendly = false;
                        }
                    }
                }

				if (NPC.localAI[1] == 300)
                    SoundEngine.PlaySound(SoundID.NPCDeath5, NPC.Center);

                if (NPC.localAI[1] >= 300 && NPC.localAI[1] <= 510)
                {
                    if (NPC.collideY && Main.rand.NextBool(6))
                    {
                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-3, -1), ModContent.ProjectileType<SnaptrapperGas>(), 12, 1, Main.myPlayer, 0, 0);
                        Main.projectile[p].hostile = true;
                        Main.projectile[p].friendly = false;
                    }
                    NPC.netUpdate = true;

                    Charging();

                    if (NPC.collideY)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + 2f), NPC.width, NPC.height, ModContent.DustType<Dusts.PoisonGas>(), NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, new Color(), 5.4f)];
                            dust.noGravity = true;
                            dust.velocity.X *= 0.3f;
                            dust.velocity.Y = (dust.velocity.Y * 0.2f) - 1;
                        }
                        frameCounter = 1;
                    }
                }
                else
                {
                    chargePhase = false;
                    NPC.aiStyle = 3;
                    AIType = NPCID.SnowFlinx;
                }

                if (NPC.localAI[1] >= 540)
                    NPC.localAI[1] = 0;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
			if (MyWorld.downedSnaptrapper)
                return spawnInfo.Player.ZoneJungle && NPC.downedBoss2 && !NPC.AnyNPCs(ModContent.NPCType<Snaptrapper>()) && spawnInfo.Player.ZoneOverworldHeight ? 0.008125f : 0f;
            return spawnInfo.Player.ZoneJungle && NPC.downedBoss2 && !NPC.AnyNPCs(ModContent.NPCType<Snaptrapper>()) && spawnInfo.Player.ZoneOverworldHeight ? 0.036f : 0f;
        }

		public void Charging()
        {
            NPC.aiStyle = -1;
            chargePhase = true;
			melee = false;

            if (NPC.ai[2] > 1f)
            {
                NPC.ai[2] -= 1f;
            }
            if (NPC.ai[2] == 0f)
            {
                NPC.ai[0] = -100f;
                NPC.ai[2] = 1f;
                NPC.TargetClosest(true);
                NPC.spriteDirection = NPC.direction;
            }

            if (NPC.wet)
            {
                if (NPC.collideX)
                {
                    NPC.direction *= -NPC.direction;
                    NPC.spriteDirection = NPC.direction;
                }
                if (NPC.collideY)
                {
                    NPC.TargetClosest(true);
					if (NPC.oldVelocity.Y < 0f)
                        NPC.velocity.Y = 5f;
                    else
                        NPC.velocity.Y = NPC.velocity.Y - 2f;
                    NPC.spriteDirection = NPC.direction;
                }
                if (NPC.velocity.Y > 4f)
                {
                    NPC.velocity.Y = NPC.velocity.Y * 0.95f;
                }
                NPC.velocity.Y = NPC.velocity.Y - 0.3f;
                if (NPC.velocity.Y < -4f)
                    NPC.velocity.Y = -4f;
            }
            if (NPC.velocity.Y == 0f)
            {
                if (NPC.ai[3] == NPC.position.X)
                {
                    NPC.direction *= -1;
                    NPC.ai[2] = 300f;
                }
                NPC.ai[3] = 0f;
                NPC.velocity.X = NPC.velocity.X * 0.9f;
                if ((double)NPC.velocity.X > -0.1 && (double)NPC.velocity.X < 0.1)
                    NPC.velocity.X = 0f;
                NPC.ai[0] += 5f;
                Vector2 vector416 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float num2721 = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - vector416.X;
                float num2720 = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - vector416.Y;
                float num2719 = (float)Math.Sqrt((double)(num2721 * num2721 + num2720 * num2720));
                float num2718 = 400f / num2719;
                num2718 = ((NPC.type != NPCID.Derpling) ? (num2718 * 10f) : (num2718 * 5f));
                if (num2718 > 30f)
                    num2718 = 30f;
                NPC.ai[0] += (float)(int)num2718;
				if (NPC.ai[0] >= 0f)
				{
					NPC.netUpdate = true;
					if (NPC.ai[2] == 1f)
						NPC.TargetClosest(true);

					float velXnum = ((NPC.life < NPC.lifeMax * .3f) ? (4.75f) : (2.525f));
					float velYnum = ((NPC.life < NPC.lifeMax * .3f) ? (10.75f) : (8.5f));

					NPC.velocity.Y = -velYnum;
					NPC.velocity.X = NPC.velocity.X + (float)(velXnum * NPC.direction);
					if (num2719 < 350f && num2719 > 200f)
						NPC.velocity.X = NPC.velocity.X + (float)NPC.direction;
					NPC.ai[0] = -120f;
					NPC.ai[1] += 1f;
				}
				else if (NPC.ai[0] >= -30f)
					NPC.aiAction = 1;

                NPC.spriteDirection = NPC.direction;
            }
            else if (NPC.target < 255)
            {
                if (NPC.direction != 1 || !(NPC.velocity.X < 3f))
                {
                    if (NPC.direction != -1 || NPC.velocity.X <= -3f)
                        return;
                }

                if (NPC.direction == -1 && NPC.velocity.X < 0.1)
                    NPC.velocity.X = NPC.velocity.X + 0.2f * NPC.direction;

                if (NPC.direction == 1 && NPC.velocity.X > -0.1)
                    NPC.velocity.X = NPC.velocity.X + 0.2f * NPC.direction;
                NPC.velocity.X = NPC.velocity.X * 0.93f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) => target.velocity.X = 11f * -target.direction;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Texture2D texture = TextureAssets.Npc[NPC.type].Value;
			Vector2 drawOrigin = NPC.frame.Size() / 2;
			int drawOffX = 8 * NPC.spriteDirection;
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			if (chargePhase)
            {
				for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + (NPC.Size / 2) + new Vector2(drawOffX, NPC.gfxOffY);
                    Color trailColor = NPC.GetAlpha(drawColor) * (float)(((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
                    spriteBatch.Draw(texture, drawPos, NPC.frame, trailColor, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
                }
            }

			Color color = NPC.GetNPCColorTintedByBuffs(drawColor);
			spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(drawOffX, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(color), NPC.rotation, drawOrigin, NPC.scale, effects, 0);
            return false;
        }

		public void RegisterToChecklist(out BossChecklistDataHandler.EntryType entryType, out float progression,
			out string name, out Func<bool> downedCondition, ref BossChecklistDataHandler.BCIDData identificationData,
			ref LocalizedText spawnInfo, ref LocalizedText despawnMessage, ref string portrait, ref string headIcon,
			ref Func<bool> isAvailable)
		{
			entryType = BossChecklistDataHandler.EntryType.Miniboss;
	        progression = 3.1f;
	        name = nameof(Snaptrapper);
	        downedCondition = () => MyWorld.downedSnaptrapper;
	        identificationData = new BossChecklistDataHandler.BCIDData(
		        new List<int> { ModContent.NPCType<Snaptrapper>() },
		        null,
		        null);
	        portrait = "SpiritMod/Textures/BossChecklist/SnaptrapperTexture";
			headIcon = Texture + "_Head_Boss";
		}
	}
}
