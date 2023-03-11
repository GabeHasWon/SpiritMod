using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.Fathomless_Chest.Entities
{
	public class Fathomless_Chest_Mimic : ModNPC
	{
		private int Counter
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private readonly int counterMax = 30;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(string.Empty);
			Main.npcFrameCount[Type] = 5;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new(0) { Hide = true };
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
			if (NPC.frameCounter >= (Main.npcFrameCount[Type] - 1) && Counter < counterMax)
			{
				float range = 130;

				if (Counter == 0)
				{
					foreach (Player player in Main.player)
					{
						if (NPC.Distance(player.Center) <= range)
						{
							RemoveCoins(player);
							break;
						}
					}
				}

				for (int i = 0; i < 2; i++)
				{
					Dust dust = Dust.NewDustPerfect(NPC.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(range - 30f, range)), DustID.GoldCoin);
					dust.velocity = dust.position.DirectionTo(NPC.Center) * Main.rand.NextFloat(2.0f, 8.0f);
					dust.noGravity = true;
				}
				Counter++;
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
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

		private void RemoveCoins(Player player)
		{
			int num1 = 0;
			for (int index = 0; index < 59; ++index)
			{
				if (player.inventory[index].type >= ItemID.CopperCoin && player.inventory[index].type <= ItemID.PlatinumCoin)
				{
					int num2 = player.inventory[index].stack / 5;
					if (Main.expertMode)
						num2 = (int)(player.inventory[index].stack * 0.25f);

					int num3 = player.inventory[index].stack - num2;
					player.inventory[index].stack -= num3;
					if (player.inventory[index].type == ItemID.CopperCoin)
						num1 += num3;

					if (player.inventory[index].type == ItemID.SilverCoin)
						num1 += num3 * 100;

					if (player.inventory[index].type == ItemID.GoldCoin)
						num1 += num3 * 10000;

					if (player.inventory[index].type == ItemID.PlatinumCoin)
						num1 += num3 * 1000000;

					if (player.inventory[index].stack <= 0)
						player.inventory[index] = new Item();

					if (index == 58)
						Main.mouseItem = player.inventory[index].Clone();
				}
			}

			player.lostCoins = num1;
			player.lostCoinString = Main.ValueToCoins(player.lostCoins);

			if (player.lostCoins > 0)
			{
				SoundEngine.PlaySound(SoundID.Coins, player.Center);
				NPC.extraValue = player.lostCoins;

				NPC.netUpdate = true;
			}
		}

		public override void FindFrame(int frameHeight)
		{
			if (Counter < counterMax)
			{
				if (NPC.frameCounter < (Main.npcFrameCount[Type] - 1))
					NPC.frameCounter += 0.2f;
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