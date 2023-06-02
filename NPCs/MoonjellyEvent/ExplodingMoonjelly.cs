using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.NPCs.Spirit;
using SpiritMod.Items.Consumable.Potion;
using Terraria.GameContent.Bestiary;
using SpiritMod.Biomes.Events;
using SpiritMod.Items.Weapon.Yoyo;

namespace SpiritMod.NPCs.MoonjellyEvent
{
	public class ExplodingMoonjelly : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moonlight Rupturer");
			Main.npcFrameCount[NPC.type] = 7;
			NPCHelper.ImmuneTo(this, BuffID.Poisoned, BuffID.Venom);
		}

		public override void SetDefaults()
		{
			NPC.width = 26;
			NPC.height = 40;
			NPC.damage = 0;
			NPC.defense = 10;
			NPC.lifeMax = 48;
			NPC.HitSound = SoundID.NPCHit25;
			NPC.DeathSound = SoundID.NPCDeath28;
            NPC.value = 10f;
			NPC.knockBackResist = .45f;
            NPC.scale = 1f;
			NPC.noGravity = true;
            NPC.noTileCollide = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.MoonlightRupturerBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<JellyDelugeBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				new FlavorTextBestiaryInfoElement("At the end of a Lunazoa's lifespan, obedient migrative drifting evolves into aggression towards foreign entities."),
			});
		}

		public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 15; k++)
            {
                Dust d = Dust.NewDustPerfect(NPC.Center, 226, Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(5), 0, default, 0.45f);
                d.noGravity = true;
            }
            if (NPC.life <= 0 && Main.netMode != NetmodeID.Server) {
                for (int k = 0; k < 30; k++)
                {
                    Dust d = Dust.NewDustPerfect(NPC.Center, 226, Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(7), 0, default, 0.65f);
                    d.noGravity = true;
                }
            }
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

        bool aggro = false;
		float alphaCounter;

		public override void AI()
        {
            alphaCounter += .04f;

			NPC.rotation = NPC.velocity.ToRotation() + 1.57f;

			if (NPC.Distance(Main.player[NPC.target].Center) <= 450 || NPC.life < NPC.lifeMax)
                aggro = true;
			else
                aggro = false;

            if (!aggro)
            {
                AIType = NPCID.Firefly;
                NPC.aiStyle = 64;
            }
			else
            {
                NPC.TargetClosest(true);

				Vector2 velocity = Main.player[NPC.target].Center - NPC.Center;
				NPC.velocity = (velocity * 0.005f) + Vector2.Normalize(velocity) * 3;

                if (Main.player[NPC.target].Hitbox.Intersects(NPC.Hitbox))
                {
                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center.X, Main.player[NPC.target].Center.Y, 0f, 0f, ModContent.ProjectileType<UnstableWisp_Explosion>(), 15, 0f, Main.myPlayer);
                    Main.projectile[p].hide = true;
                    Main.projectile[p].timeLeft = 20;

                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                }
            }
            Lighting.AddLight(NPC.Center, 0.075f * 2, 0.231f * 2, 0.255f * 2);
        }

        public override bool CheckDead()
        {
            for (int k = 0; k < 30; k++)
            {
                Dust d = Dust.NewDustPerfect(NPC.Center, 226, Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(7), 0, default, 0.75f);
                d.noGravity = true;
            }
            return true;
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon(ItemID.Gel, 1, 1, 3);
			npcLoot.AddCommon<MoonJelly>(5);
			npcLoot.AddCommon<Moonburst>(35);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Main.spriteBatch.Draw(
				Mod.Assets.Request<Texture2D>("NPCs/MoonjellyEvent/ExplodingMoonjelly_Glow").Value,
				NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY),
				NPC.frame,
				Color.White,
				NPC.rotation,
				NPC.frame.Size() / 2,
				NPC.scale,
				SpriteEffects.None,
				0
			);
        }
    }
}
