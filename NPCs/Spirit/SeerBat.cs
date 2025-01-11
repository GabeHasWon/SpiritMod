using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Equipment;
using SpiritMod.Items.Sets.SpiritSet;
using SpiritMod.Tiles.Block;
using SpiritMod.Utilities;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Spirit
{
	public class SeerBat : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spirit Bat");
			Main.npcFrameCount[NPC.type] = 3;
		}

		public override void SetDefaults()
		{
			NPC.width = 51;
			NPC.height = 28;
			NPC.damage = 44;
			NPC.defense = 19;
			NPC.lifeMax = 240;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath4;
			NPC.value = 60f;
			NPC.knockBackResist = .45f;
			NPC.aiStyle = 14;

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.SpiritBatBanner>();
			AIType = NPCID.CaveBat;
			SpawnModBiomes = [ModContent.GetInstance<Biomes.SpiritUndergroundBiome>().Type];
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
			Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0f, 0.135f, 0.75f);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;

            if (player.ZoneSpirit() && spawnInfo.Player.Center.Y / 16 > Main.worldSurface && !spawnInfo.PlayerInTown && !spawnInfo.Invasion)
			{
				int[] spawnTiles = { ModContent.TileType<SpiritDirt>(), ModContent.TileType<SpiritStone>(), ModContent.TileType<Spiritsand>(), ModContent.TileType<SpiritIce>(), };
				return spawnTiles.Contains(spawnInfo.SpawnTileType) ? 6.09f : 0f;
			}
			return 0f;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			var pos = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY);
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/Spirit/SeerBat_Glow").Value, screenPos);

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server) {
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 13);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 12);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 11);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon(ModContent.ItemType<SoulShred>(), 3);
			npcLoot.AddCommon(ModContent.ItemType<SpiritOre>(), 1, 1, 2);
		}
	}
}
