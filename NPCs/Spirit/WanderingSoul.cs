using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Tiles.Block;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.SpiritBiomeDrops;
using Terraria.GameContent.Bestiary;
using SpiritMod.Utilities;
using Terraria.ModLoader.Utilities;

namespace SpiritMod.NPCs.Spirit
{
	public class WanderingSoul : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Wandering Soul");
			Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.Wraith];
			NPCHelper.BuffImmune(Type, true);
		}

		public override void SetDefaults()
		{
			NPC.width = 34;
			NPC.height = 48;
			NPC.damage = 37;
			NPC.defense = 40;
			NPC.lifeMax = 340;
			NPC.HitSound = SoundID.NPCHit3;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 60f;
			NPC.knockBackResist = .65f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = 22;
			NPC.stepSpeed = .5f;

			AIType = NPCID.Wraith;
			AIType = NPCID.Wraith;
			AnimationType = NPCID.Wraith;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.WanderingSoulBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiritUndergroundBiome>().Type };
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
			=> GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/Spirit/WanderingSoul_Glow").Value, screenPos);

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;

			if (player.ZoneSpirit() && !spawnInfo.PlayerSafe && spawnInfo.SpawnTileY < Main.rockLayer && !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust) && (!(Main.pumpkinMoon || Main.snowMoon || Main.eclipse) || spawnInfo.Player.Center.Y / 16 > Main.worldSurface) && SpawnCondition.GoblinArmy.Chance == 0 && !spawnInfo.Invasion)
			{
				int[] spawnTiles = { ModContent.TileType<SpiritDirt>(), ModContent.TileType<SpiritStone>(), ModContent.TileType<Spiritsand>(), ModContent.TileType<SpiritGrass>(), ModContent.TileType<SpiritIce>() };
				return spawnTiles.Contains(spawnInfo.SpawnTileType) ? 1.59f : 0f;
			}
			return 0f;
		}

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
			npcLoot.AddCommon<Items.Sets.RunicSet.Rune>(3);
			npcLoot.AddCommon<StoneOfSpiritsPast>(100);
		}
	}
}