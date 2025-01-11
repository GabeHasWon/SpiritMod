using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Buffs;
using SpiritMod.Buffs.DoT;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.BlizzardBandit
{
    public class BlizzardBandit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;
			NPCHelper.ImmuneTo(this, BuffID.Frostburn, ModContent.BuffType<MageFreeze>(), ModContent.BuffType<CryoCrush>());

			var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Position = new Vector2(-4, 0),
				Velocity = 1f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

        int timer = 0;
        bool shooting = false;
        bool gettingballs = false;

        public override void SetDefaults()
        {
            NPC.aiStyle = 3;
            NPC.lifeMax = 55;
            NPC.defense = 6;
            NPC.value = 65f;
            NPC.knockBackResist = 0.7f;
            NPC.width = 30;
			NPC.height = 54;
            NPC.damage = 15;
            NPC.lavaImmune = false;
            NPC.noTileCollide = false;
            NPC.alpha = 0;
            NPC.dontTakeDamage = false;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCHit1;

            AIType = NPCID.SnowFlinx;
            Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BlizzardBanditBanner>();
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Snow Blizzard");

		public override bool PreAI()
        {
            if (gettingballs)
            {
                NPC.velocity.Y = 6;
                NPC.velocity.X *= 0.08f;
            }

            if (timer == 240)
            {
                shooting = true;
                gettingballs = true;
                timer = 0;
            }

            if (!shooting)
                timer++;

            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = -1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = 1;
            return base.PreAI();
        }

        public override void AI()
        {
            if (shooting)
            {
                if (NPC.velocity.X < 0f)
                    NPC.spriteDirection = -1;
                else if (NPC.velocity.X > 0f)
                    NPC.spriteDirection = 1;
            }

			if ((frame == 11 || frame == 14) && NPC.frameCounter == 4)
			{
				SoundEngine.PlaySound(SoundID.Item19 with { Volume = 0.5f }, NPC.Center);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 direction = NPC.DirectionTo(Main.player[NPC.target].Center) * 8.5f;
					float A = Main.rand.Next(-50, 50) * 0.02f;
					float B = Main.rand.Next(-50, 50) * 0.02f;
					int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + (NPC.direction * 12), NPC.Center.Y, direction.X + A, direction.Y + B, ProjectileID.SnowBallFriendly, 5, 1, Main.myPlayer, 0, 0);
					Main.projectile[p].hostile = true;
					Main.projectile[p].friendly = false;
				}
			}
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 30; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Rope, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, Main.rand.NextFloat(.3f, 1.1f));

            if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
				for (int i = 1; i < 5; ++i)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("BlizzardBanditGore" + i).Type, 1f);
        }

        int frame = 0;

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (!shooting)
            {
                if (NPC.frameCounter >= 7)
                {
                    frame++;
                    NPC.frameCounter = 0;
                }

                if (frame >= 6)
                    frame = 0;
            }
            else
            {
                if (Main.player[NPC.target].Center.X < NPC.Center.X)
                    NPC.spriteDirection = -1;
                else
                    NPC.spriteDirection = 1;

                if (NPC.frameCounter >= 7)
                {
                    frame++;
                    NPC.frameCounter = 0;
                }

                if (frame == 10)
                    gettingballs = false;

                if (frame >= 16)
                {
                    shooting = false;
                    frame = 0;
                }

                if (frame < 6)
                    frame = 6;
            }

            NPC.frame.Y = frameHeight * frame;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(timer);
			writer.Write(shooting);
			writer.Write(gettingballs);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			timer = reader.ReadInt32();
			shooting = reader.ReadBoolean();
			gettingballs = reader.ReadBoolean();
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneSnow && spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !spawnInfo.PlayerSafe
			&& !spawnInfo.Invasion ? 0.0895f : 0f;

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Snowball, 1, 8, 13));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Armor.Masks.WinterHat>(), 20));
		}
	}
}