using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.SpiritSet;
using SpiritMod.Items.Sets.SpiritBiomeDrops;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using SpiritMod.Tiles.Block;
using System.Linq;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using SpiritMod.Utilities;
using SpiritMod.Biomes;

namespace SpiritMod.NPCs.Spirit
{
	public class SoulCrusher : ModNPC
	{
		int frame = 0;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Soul Crusher");
			Main.npcFrameCount[NPC.type] = 7;
			NPCHelper.ImmuneTo(this, BuffID.OnFire);
		}

		public override void SetDefaults()
		{
			NPC.width = 64;
			NPC.height = 52;
			NPC.damage = 35;
			NPC.defense = 15;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit3;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 60f;
			NPC.knockBackResist = .35f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.lavaImmune = true;

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.SoulCrusherBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<SpiritUndergroundBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			var pos = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY);
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
			=> GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/Spirit/SoulCrusher_Glow").Value, screenPos);

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;

			if (player.ZoneSpirit() && spawnInfo.SpawnTileY > SpiritUndergroundBiome.ThirdLayerHeight && !spawnInfo.PlayerSafe && !spawnInfo.Invasion)
			{
				int[] spawnTiles = new int[] { ModContent.TileType<SpiritDirt>(), ModContent.TileType<SpiritStone>(), ModContent.TileType<SpiritGrass>(), ModContent.TileType<SpiritIce>() };
				return spawnTiles.Contains(spawnInfo.SpawnTileType) ? 3f : 0f;
			}
			return 0f;
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 13);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 12);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 11);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon(ModContent.ItemType<Gravehunter>(), 75);
			npcLoot.AddCommon(ModContent.ItemType<SoulShred>(), 2);
			npcLoot.AddCommon(ModContent.ItemType<SpiritOre>(), 1, 1, 2);
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += .07f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override bool PreAI()
		{
			Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.05f, 0.05f, 0.4f);
			NPC.TargetClosest(true);
			Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
			NPC.rotation = direction.ToRotation();
			direction.Normalize();
			NPC.velocity *= 0.98f;
			int dust2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.UnusedWhiteBluePurple, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
			Main.dust[dust2].noGravity = true;

			if (frame == 0) {
				direction.X = direction.X * Main.rand.Next(10, 15);
				direction.Y = direction.Y * Main.rand.Next(10, 15);
				NPC.velocity.X = direction.X;
				NPC.velocity.Y = direction.Y;
			}
			return false;
		}
	}
}
