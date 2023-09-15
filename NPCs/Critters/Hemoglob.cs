using Microsoft.Xna.Framework;
using SpiritMod.Items.Consumable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.Critters
{
	public class Hemoglob : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Hemoglob");
			Main.npcFrameCount[NPC.type] = 7;
			Main.npcCatchable[NPC.type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 16;
			NPC.height = 12;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 5;
			NPC.dontCountMe = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.catchItem = (short)ModContent.ItemType<HemoglobItem>();
			NPC.knockBackResist = .45f;
			NPC.aiStyle = 7;
			NPC.npcSlots = 0;
			NPC.noGravity = false; ;
			AIType = NPCID.Bunny;
			NPC.dontTakeDamageFromHostiles = false;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "TheCrimson");

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				for (int k = 0; k < 20; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 1.75f * hit.HitDirection, -1.75f, 0, new Color(), 0.86f);
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneOverworldHeight ? .06f : 0f;
		public override void AI() => NPC.spriteDirection = NPC.direction;

		public override void FindFrame(int frameHeight)
		{
			if (NPC.velocity != Vector2.Zero || NPC.IsABestiaryIconDummy)
			{
				NPC.frameCounter += 0.25f;
				NPC.frameCounter %= Main.npcFrameCount[NPC.type];
				int frame = (int)NPC.frameCounter;
				NPC.frame.Y = frame * frameHeight;
			}
			else
			{
				int frame = (int)NPC.frameCounter;
				NPC.frame.Y = frame * 0;
			}
		}
	}
}