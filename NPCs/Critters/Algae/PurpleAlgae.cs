using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Critters.Algae
{
	public class PurpleAlgae2 : ModNPC
	{
		private ref float DespawnTimer => ref NPC.ai[0];
		private ref float LightTimer => ref NPC.ai[1];

		bool collision = false;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 1;

			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new() { Hide = true };
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

		public override void OnSpawn(IEntitySource source)
		{
			if (NPC.type == ModContent.NPCType<PurpleAlgae2>())
			{
				for (int i = 0; i < 5; ++i)
				{
					Vector2 dir = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);
					string[] npcChoices = { "PurpleAlgae1", "PurpleAlgae3" };
					int npcChoice = Main.rand.Next(npcChoices.Length);
					int newNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (Main.rand.Next(-55, 55)), (int)NPC.Center.Y + (Main.rand.Next(-20, 20)), Mod.Find<ModNPC>(npcChoices[npcChoice]).Type, NPC.whoAmI);
					Main.npc[newNPC].velocity.X = dir.X;
				}
			}
		}

		public override bool PreAI()
		{
			if (Main.dayTime)
			{
				DespawnTimer++;
				if (DespawnTimer >= Main.rand.Next(100, 700))
				{
					NPC.active = false;
					NPC.netUpdate = true;
				}
			}
			return true;
		}

		public override void AI()
		{
			if (++LightTimer >= Main.rand.Next(100, 400))
				LightTimer = 0;

			if (!Main.dayTime)
				Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0.208f * 2, 0.107f * 2, .255f * 2);

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

			if (!NPC.wet)
				NPC.velocity.Y += 0.2f;
			else if (Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y - 6) / 16)].LiquidAmount > 150)
				NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, -5, 0.05f);
			else
				NPC.velocity.Y *= 0.95f;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			drawColor = new Color(224 - (int)(LightTimer / 3 * 4), 158 - (int)(LightTimer / 3 * 4), 255 - (int)(LightTimer / 3 * 4), 255 - LightTimer);
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			var pos = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);

			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}
	}

	public class PurpleAlgae1 : PurpleAlgae2
	{ 
	}

	public class PurpleAlgae3 : PurpleAlgae2
	{
	}
}