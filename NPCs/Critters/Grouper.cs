using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Consumable.Fish;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.Critters
{
	public class Grouper : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Grouper");
			Main.npcFrameCount[NPC.type] = 4;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 52;
			NPC.height = 22;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 5;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = .35f;
			NPC.aiStyle = 16;
			NPC.noGravity = true;
			NPC.dontCountMe = true;
			NPC.npcSlots = 0;
			AIType = NPCID.Goldfish;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Ocean");

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void AI()
		{
			Player target = Main.player[NPC.target];
			if (NPC.DistanceSQ(target.Center) < 65 * 65 && target.wet && NPC.wet)
			{
				Vector2 vel = NPC.DirectionFrom(target.Center);
				vel.Normalize();
				vel *= 4.5f;
				NPC.velocity = vel;
				NPC.rotation = NPC.velocity.X * .06f;
				if (target.position.X > NPC.position.X)
				{
					NPC.spriteDirection = -1;
					NPC.direction = -1;
					NPC.netUpdate = true;
				}
				else if (target.position.X < NPC.position.X)
				{
					NPC.spriteDirection = 1;
					NPC.direction = 1;
					NPC.netUpdate = true;
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life > 0 || Main.netMode == NetmodeID.Server)
				return;

			for (int i = 0; i < 30; i++)
			{
				if (i < 3)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Grouper" + (i + 1)).Type, 1f);

				int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 0.5f;
				Main.dust[dust].scale *= .6f;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddCommon<RawFish>();
		public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneBeach && spawnInfo.Water ? 0.029f : 0f;
	}
}