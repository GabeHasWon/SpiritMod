using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.Fathomless_Chest.Entities
{
	public class Fathomless_Chest_Gold : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault(string.Empty);
			Main.npcFrameCount[Type] = 6;

			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
		}

		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 48;
			NPC.damage = 0;
			NPC.dontCountMe = true;
			NPC.defense = 0;
			NPC.lifeMax = 1;
			NPC.aiStyle = -1;
			NPC.npcSlots = 0;
			NPC.knockBackResist = 0f;
			NPC.DeathSound = SoundID.DD2_SkeletonDeath;
			NPC.dontTakeDamageFromHostiles = true;

			DrawOffsetY = -2;
		}

		public override void AI()
		{
			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(NPC.position + new Vector2(0, NPC.gfxOffY), NPC.width, NPC.height, DustID.GoldCoin, 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1.0f));
				dust.noGravity = true;
				if (Main.rand.NextBool(2))
					dust.fadeIn = 1.1f;
				dust.velocity = Vector2.Zero;
			}

			Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), .4f, .4f, .2f);
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life > 0)
				return;

			for (int g = 0; g < 5; g++)
			{
				Gore gore = Gore.NewGoreDirect(Entity.GetSource_Death(), NPC.Center - new Vector2(0, 18), Vector2.Zero, Mod.Find<ModGore>("FathomlessChest_Gold" + (g + 1)).Type, 1f);
				gore.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.5f, 1.8f);
			}
		}

		public override void FindFrame(int frameHeight)
		{
			if (NPC.frameCounter < (Main.npcFrameCount[Type] - 1))
				NPC.frameCounter += 0.2f;
			NPC.frame.Y = frameHeight * (int)NPC.frameCounter;
		}
	}
}