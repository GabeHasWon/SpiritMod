using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Equipment;
using SpiritMod.Tiles.Block;
using SpiritMod.Utilities;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace SpiritMod.NPCs.Spirit
{
	public class SpiritMummy : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dusk Mummy");
			Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.Mummy];

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { Velocity = 1f };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 34;
			NPC.height = 48;
			NPC.damage = 50;
			NPC.defense = 20;
			NPC.lifeMax = 220;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 60f;
			NPC.knockBackResist = .20f;
			NPC.aiStyle = 3;
			AIType = NPCID.Mummy;
			AnimationType = NPCID.Mummy;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiritSurfaceBiome>().Type };

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.SpiritMummyBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;

			if (player.ZoneSpirit() && spawnInfo.SpawnTileY < Main.rockLayer && !spawnInfo.PlayerInTown && !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust) && (!(Main.pumpkinMoon || Main.snowMoon || Main.eclipse) || spawnInfo.Player.Center.Y / 16 > Main.worldSurface) && SpawnCondition.GoblinArmy.Chance == 0)
			{
				int[] spawnTiles = { ModContent.TileType<Spiritsand>() };
				return spawnTiles.Contains(spawnInfo.SpawnTileType) ? 5f : 0f;
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

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			if (Main.rand.NextBool(5))
				target.AddBuff(BuffID.Cursed, 150);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon(ModContent.ItemType<Items.Sets.RunicSet.Rune>(), 3);
			npcLoot.AddCommon(ModContent.ItemType<Obolos>(), 300);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Rectangle drawFrame = NPC.frame with { Height = NPC.frame.Height - 2 };
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), drawFrame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, ModContent.Request<Texture2D>(Texture + "_Glow").Value, screenPos);
	}
}