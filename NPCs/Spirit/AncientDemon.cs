using Microsoft.Xna.Framework;
using SpiritMod.Items.Sets.SpiritSet;
using Terraria;
using Terraria.ID;
using SpiritMod.Tiles.Block;
using System.Linq;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using SpiritMod.Biomes;
using SpiritMod.Utilities;
using SpiritMod.Items.Equipment;

namespace SpiritMod.NPCs.Spirit;

public class AncientDemon : ModNPC
{
	private ref float Timer => ref NPC.ai[3];

	public override void SetDefaults()
	{
		NPC.width = 80;
		NPC.height = 80;
		NPC.damage = 40;
		NPC.defense = 19;
		NPC.lifeMax = 400;
		NPC.HitSound = SoundID.NPCHit2;
		NPC.DeathSound = SoundID.NPCDeath6;
		NPC.value = Item.buyPrice(0, 0, 30, 0);
		NPC.knockBackResist = .05f;
		NPC.aiStyle = 44;
		NPC.noGravity = true;
		NPC.noTileCollide = true;

		Banner = NPC.type;
		BannerItem = ModContent.ItemType<Items.Banners.AncientSpectreBanner>();
		SpawnModBiomes = [ModContent.GetInstance<SpiritUndergroundBiome>().Type];
		AIType = NPCID.FlyingAntlion;
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

	public override void AI()
	{
		Timer++;

		Lighting.AddLight(NPC.Center, 0f, 0.0675f, 0.250f);

		if (Timer % 240 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
		{
			Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 4f;

			float A = Main.rand.Next(-1, 1) * 0.01f;
			float B = Main.rand.Next(-1, 1) * 0.01f;
			int damage = NPCUtils.ToActualDamage(30, 1.5f, 2f);
			Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, direction.X + A, direction.Y + B, ModContent.ProjectileType<SpiritScythe>(), damage, 1, Main.myPlayer, 0, 0);
		}

		NPC.spriteDirection = NPC.direction;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Asphalt, 0f, 0f, 100, default, 1f);

		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
		{
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 13);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 12);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 11);

			for (int num621 = 0; num621 < 20; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Asphalt, 0f, 0f, 100, default, 2f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.NextBool(2))
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}

			for (int num623 = 0; num623 < 40; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Asphalt, 0f, 0f, 100, default, 3f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Asphalt, 0f, 0f, 100, default, 2f);
				Main.dust[num624].velocity *= 2f;
			}
		}
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		Player player = spawnInfo.Player;

		if (player.ZoneSpirit() && spawnInfo.SpawnTileY > SpiritUndergroundBiome.ThirdLayerHeight && !spawnInfo.PlayerSafe)
		{
			int[] spawnTiles = [ModContent.TileType<SpiritDirt>(), ModContent.TileType<SpiritStone>(), ModContent.TileType<SpiritGrass>(), ModContent.TileType<SpiritIce>()];
			return spawnTiles.Contains(spawnInfo.SpawnTileType) ? 0.8f : 0f;
		}
		return 0f;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.AddCommon(ModContent.ItemType<SoulShred>(), 3);
		npcLoot.AddCommon(ModContent.ItemType<Obolos>(), 300);
	}
}