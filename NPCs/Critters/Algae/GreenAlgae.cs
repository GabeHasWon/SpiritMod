using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Critters.Algae
{
	public class GreenAlgae2 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bioluminescent Algae");
			Main.npcFrameCount[NPC.type] = 1;

			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new(0)
			{
				Hide = true
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
		}

		public override void SetDefaults()
		{
			NPC.width = 6;
			NPC.height = 6;
			NPC.damage = 0;
			NPC.defense = 1000;
			NPC.lifeMax = 1;
			NPC.aiStyle = -1;
			NPC.npcSlots = 0;
			NPC.noGravity = true;
			NPC.alpha = 40;
			NPC.behindTiles = true;
			NPC.dontCountMe = true;
			NPC.dontTakeDamage = true;
		}


		public float num42;
		int num = 0;
		bool collision = false;
		int num1232;

		public override void OnSpawn(IEntitySource source)
		{
			int npcXTile = (int)(NPC.Center.X / 16);
			int npcYTile = (int)(NPC.Center.Y / 16);
			for (int y = npcYTile; y > Math.Max(0, npcYTile - 100); y--)
			{
				if (Main.tile[npcXTile, y].LiquidAmount != 255)
				{
					int liquid = Main.tile[npcXTile, y].LiquidAmount;
					float up = (liquid / 255f) * 16f;
					NPC.position.Y = (y + 1) * 16f - up + 8;
					break;
				}
			}

			if (NPC.type == ModContent.NPCType<GreenAlgae2>())
			{
				for (int i = 0; i < 5; ++i)
				{
					Vector2 dir = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);
					string[] npcChoices = { "GreenAlgae1", "GreenAlgae3" };
					int npcChoice = Mod.Find<ModNPC>(npcChoices[Main.rand.Next(npcChoices.Length)]).Type;
					int newNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (Main.rand.Next(-55, 55)), (int)NPC.Center.Y + (Main.rand.Next(-20, 20)), npcChoice, NPC.whoAmI);
					Main.npc[newNPC].velocity.X = dir.X;
				}
			}
		}

		public override bool PreAI()
		{
			if (Main.dayTime)
			{
				num1232++;
				if (num1232 >= Main.rand.Next(100, 700))
				{
					NPC.active = false;
					NPC.netUpdate = true;
				}
			}
			return true;
		}

		public override void AI()
		{
			num++;
			if (num >= Main.rand.Next(100, 400))
				num = 0;

			if (!Main.dayTime)
				Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0.135f * 2, 0.255f * 2, .211f * 2);

			NPC.spriteDirection = -NPC.direction;

			if (!collision)
				NPC.velocity.X = .5f * Main.windSpeedCurrent;
			else
				NPC.velocity.X = -.5f * Main.windSpeedCurrent;

			if (NPC.collideX || NPC.collideY)
			{
				NPC.velocity.X *= -1f;
				collision = !collision;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			drawColor = new Color(176 - (num / 3 * 4), 255 - (num / 3 * 4), 237 - (num / 3 * 4), 255 - num);
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			var pos = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY - 8);

			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}
	}

	public class GreenAlgae1 : GreenAlgae2
	{
	}

	public class GreenAlgae3 : GreenAlgae2
	{
	}
}