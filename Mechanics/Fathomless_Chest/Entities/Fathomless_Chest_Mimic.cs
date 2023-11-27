using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.Fathomless_Chest.Entities
{
	public class Fathomless_Chest_Mimic : ModNPC
	{
		private readonly List<Item> stored = new();

		public ref float Counter => ref NPC.ai[0];

		private const int IDLE_TIME = 20;
		private const int OPEN_TIME = 30;

		public override LocalizedText DisplayName => Language.GetText("NPCName.Mimic");

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 5;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new() { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
		}

		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 48;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 100;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0f;
			NPC.DeathSound = SoundID.DD2_SkeletonDeath;
			NPC.HitSound = SoundID.DD2_WitherBeastHurt;
			DrawOffsetY = -2;
		}

		public override void AI()
		{
			NPC.TargetClosest(false);
			Player player = Main.player[NPC.target];

			if (++Counter > IDLE_TIME && Counter < (IDLE_TIME + OPEN_TIME))
			{
				float range = 130;

				if (Counter == (IDLE_TIME + 1))
					if (NPC.Distance(player.Center) <= range)
						RemoveCoins(player);

				for (int i = 0; i < 2; i++)
				{
					Dust dust = Dust.NewDustPerfect(NPC.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(range - 30f, range)), DustID.GoldCoin);
					dust.velocity = dust.position.DirectionTo(NPC.Center) * Main.rand.NextFloat(2.0f, 8.0f);
					dust.noGravity = true;
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			if (NPC.life <= 0)
			{
				for (int g = 0; g < 6; g++)
				{
					Gore gore = Gore.NewGoreDirect(Entity.GetSource_Death(), NPC.Center, Vector2.Zero, 99, 1.1f);
					gore.velocity *= 0.6f;

					if (g < 5)
					{
						gore = Gore.NewGoreDirect(Entity.GetSource_Death(), NPC.Center - new Vector2(0, 18), Vector2.Zero, Mod.Find<ModGore>("FathomlessChest" + (g + 1)).Type, 1f);
						gore.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.5f, 1.8f);
					}
				}
			}
			else
			{
				for (int d = 0; d < 6; d++)
					Dust.NewDust(NPC.position + new Vector2(0, NPC.gfxOffY), NPC.width, NPC.height, DustID.DungeonSpirit);
			}
		}

		public override void OnKill()
		{
			foreach (Item item in stored)
			{
				int id = Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), item);

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.SyncItem, number: id);
			}
		}

		public override bool CheckActive() => !stored.Any();

		private void RemoveCoins(Player player)
		{
			float mult = Main.masterMode ? 1f : Main.expertMode ? .75f : .4f;

			for (int i = 0; i < Main.InventorySlotsTotal; ++i)
			{
				int stack = (int)(player.inventory[i].stack * mult);
				if (player.inventory[i].IsACoin)
				{
					Item clone = player.inventory[i].Clone();
					clone.stack = stack;

					if ((player.inventory[i].stack -= stack) <= 0)
						player.inventory[i].TurnToAir();

					stored.Add(clone);
				}
			}

			if (stored.Any())
				SoundEngine.PlaySound(SoundID.Coins, player.Center);
		}

		public override void FindFrame(int frameHeight)
		{
			if (Counter < (IDLE_TIME + OPEN_TIME))
			{
				if (NPC.frameCounter < (Main.npcFrameCount[Type] - 1))
					NPC.frameCounter += Main.npcFrameCount[Type] / (float)IDLE_TIME;
			}
			else
			{
				if (NPC.frameCounter > 0)
					NPC.frameCounter -= 0.2f;
			}

			NPC.frame.Y = frameHeight * (int)NPC.frameCounter;
		}
	}
}