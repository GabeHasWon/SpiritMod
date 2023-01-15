using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace SpiritMod.NPCs.Boss.Scarabeus
{
	public class Scarab : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scarab");
			Main.npcFrameCount[Type] = 4;
			NPCID.Sets.CountsAsCritter[Type] = true;

			var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Velocity = 1f };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 20;
			NPC.height = 20;
			NPC.defense = 0;
			NPC.lifeMax = 5;
			NPC.dontCountMe = true;
			NPC.npcSlots = 0f;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 0f;
			NPC.knockBackResist = 0f;
			NPC.aiStyle = 7;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], true);

			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
				new FlavorTextBestiaryInfoElement("The scrappy brethren of the larger Scarabeus, these little things follow their leader to the death."),
			});
		}

		public override bool PreAI()
		{
			NPC.spriteDirection = NPC.direction;
			return true;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);

			if (NPC.life <= 0)
			{
				for (int k = 0; k < 10; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);

				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("LittleScarab4").Type, 1f);
			}
		}

		public override void FindFrame(int frameHeight)
		{
			if (NPC.velocity.X != 0)
				NPC.frameCounter += 0.25f;
			else
				NPC.frameCounter = 0;

			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}
	}

	public class Scarab_Wall : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scarab");
			Main.npcFrameCount[Type] = 8;
			NPCID.Sets.CountsAsCritter[Type] = true;

			var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 20;
			NPC.height = 20;
			NPC.lifeMax = 5;
			NPC.dontCountMe = true;
			NPC.npcSlots = 0f;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 0f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Butterfly;
		}

		public override void OnSpawn(IEntitySource source) => NPC.scale = 1f;

		public override void AI()
		{
			if (NPC.scale != 1f)
				NPC.scale = 1f;

			Point16 tilePos = new Point16((int)(NPC.Center.X / 16), (int)(NPC.Center.Y / 16));

			NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
			NPC.velocity *= .92f;

			if (Framing.GetTileSafely(tilePos.X, tilePos.Y).WallType == WallID.None)
				NPC.Transform(ModContent.NPCType<Scarab>());
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);

			if (NPC.life <= 0)
			{
				for (int k = 0; k < 10; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);

				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("LittleScarab4").Type, 1f);
			}
		}

		public override void FindFrame(int frameHeight)
		{
			if (NPC.velocity != Vector2.Zero)
				NPC.frameCounter += 0.25f;
			else
				NPC.frameCounter = 0;

			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}
	}
}