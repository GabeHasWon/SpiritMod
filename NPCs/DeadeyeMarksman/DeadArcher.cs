using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Material;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.DeadeyeMarksman
{
	public class DeadArcher : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.GoblinArcher];

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { Velocity = 1f };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 36;
			NPC.height = 46;
			NPC.damage = 0;
			NPC.defense = 9;
			NPC.lifeMax = 47;
			if (Main.expertMode)
                NPC.lifeMax = 94;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 120f;
			NPC.knockBackResist = .30f;
			NPC.aiStyle = 3;
			AIType = NPCID.GoblinArcher;
			AnimationType = NPCID.GoblinArcher;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.DeadeyeMarksmanBanner>();
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Surface NightTime");

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<OldLeather>();
			npcLoot.AddCommon(ItemID.BlackLens, 100);
			npcLoot.AddCommon(ItemID.WoodenArrow, 1, 5, 9);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			bool conditions = spawnInfo.SpawnTileY < Main.rockLayer && !Main.dayTime && !spawnInfo.PlayerSafe && spawnInfo.Player.ZoneOverworldHeight 
				&& !spawnInfo.Player.ZoneDesert && !spawnInfo.Player.ZoneCorrupt && !spawnInfo.Player.ZoneCrimson && !spawnInfo.Player.ZoneBeach && !spawnInfo.Player.ZoneJungle 
				&& !Main.pumpkinMoon && !Main.snowMoon && !spawnInfo.PlayerInTown;

			if (!NPC.downedBoss1)
				return conditions ? 0.025f : 0f;
			if (Main.hardMode)
				return conditions ? 0.01f : 0f;
			return conditions ? 0.04f : 0f;
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) => NPC.lifeMax = (int)(NPC.lifeMax * balance);

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 40; k++) {
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, .45f);
			}
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server) {
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Archer2").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Archer1").Type, 1f);
				for (int k = 0; k < 80; k++) {
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, .85f);
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/DeadeyeMarksman/DeadArcher_Glow").Value, screenPos);

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			if (Main.rand.NextBool(4))
				target.AddBuff(BuffID.Darkness, 180);
		}
	}
}
