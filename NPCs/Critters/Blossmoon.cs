using Microsoft.Xna.Framework;
using SpiritMod.Items.Consumable;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.QuestSystem.Quests;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.Critters
{
	public class Blossmoon : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blossmoon");
			Main.npcFrameCount[NPC.type] = 45;
			Main.npcCatchable[NPC.type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 18;
			NPC.height = 20;
			NPC.damage = 0;
			NPC.dontCountMe = true;
			NPC.defense = 0;
			NPC.lifeMax = 5;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.catchItem = (short)ModContent.ItemType<BlossmoonItem>();
			NPC.dontTakeDamageFromHostiles = false;
			NPC.knockBackResist = .45f;
			NPC.aiStyle = 0;
			NPC.npcSlots = 0;
			NPC.noGravity = false;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "NightTime");

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 30; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.ShadowbeamStaff, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, Main.rand.NextFloat(.2f, .8f));
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			bool valid = spawnInfo.SpawnTileY < Main.rockLayer && !Main.dayTime && MyWorld.calmNight && !spawnInfo.Invasion && !spawnInfo.Sky && !Main.eclipse;

			if (!valid)
				return 0f;

			return QuestManager.GetQuest<CritterCaptureBlossmoon>().IsActive && !NPC.AnyNPCs(NPC.type) ? 0.4f : 0.1f;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override bool PreAI()
		{
			Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), .92f / 2, .632f / 2, 1.71f / 2);
			Player target = Main.player[NPC.target];
			if (NPC.DistanceSQ(target.Center) < 400 * 400)
				target.AddBuff(BuffID.Calm, 300);
			return true;
		}
	}
}
