﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Accessory;
using System;
using SpiritMod.Items.Armor.AstronautVanity;
using Terraria.GameContent.Bestiary;
using SpiritMod.Biomes;
using Terraria.GameContent.ItemDropRules;

namespace SpiritMod.NPCs.Gloop
{
	public class GloopGloop : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gloop");
			Main.npcFrameCount[NPC.type] = 3;
			NPCHelper.ImmuneTo(this, BuffID.Poisoned, BuffID.Confused);
		}

		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 48;
			NPC.damage = 28;
			NPC.defense = 8;
			NPC.lifeMax = 85;
			NPC.noGravity = true;
			NPC.value = 90f;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.DD2_GoblinHurt;
			NPC.DeathSound = SoundID.NPCDeath22;
			NPC.noGravity = true;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.GloopBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<AsteroidBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		int xoffset = 0;

		public override void AI()
		{
			NPC.TargetClosest(false);
			Player player = Main.player[NPC.target];
			NPC.ai[0]++;

			if(player.position.X > NPC.position.X) {
				xoffset = 16;
			} else {
				xoffset = -16;
			}
			NPC.velocity.X *= 0.99f;
				if(NPC.ai[1] == 0) {
					if(NPC.velocity.Y < 2.5f) {
						NPC.velocity.Y += 0.1f;
					}
					if(player.position.Y < NPC.position.Y && NPC.ai[0] % 30 == 0) {
						NPC.ai[1] = 1;
						NPC.netUpdate = true;
						NPC.velocity.X = xoffset / 1.25f;
						NPC.velocity.Y = -6;
					}
				}
				if(NPC.ai[1] == 1) {
					NPC.velocity *= 0.97f;
					if(Math.Abs(NPC.velocity.X) < 0.125f) {
						NPC.ai[1] = 0;
						NPC.netUpdate = true;
					}
					NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
				}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<GravityModulator>(400);
			npcLoot.AddOneFromOptions(67, ModContent.ItemType<AstronautHelm>(), ModContent.ItemType<AstronautBody>(), ModContent.ItemType<AstronautLegs>());
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++) {
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Plantera_Green, hit.HitDirection, -1f, 0, default, .61f);
			}
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server) {
				for (int k = 0; k < 20; k++) {
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Plantera_Green, hit.HitDirection, -1f, 0, default, .91f);
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

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			if (Main.rand.NextBool(6)) {
				target.AddBuff(BuffID.Poisoned, 180);
			}
		}
	}
}
