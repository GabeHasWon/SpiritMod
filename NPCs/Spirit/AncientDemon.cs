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

namespace SpiritMod.NPCs.Spirit
{
	public class AncientDemon : ModNPC
	{
		int timer = 0;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Ancient Specter");

		public override void SetDefaults()
		{
			NPC.width = 80;
			NPC.height = 80;
			NPC.damage = 32;
			NPC.defense = 19;
			NPC.lifeMax = 300;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 460f;
			NPC.knockBackResist = .15f;
			NPC.aiStyle = 44;
			NPC.noGravity = true;
			NPC.noTileCollide = true;

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.AncientSpectreBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<SpiritUndergroundBiome>().Type };
			AIType = NPCID.FlyingAntlion;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				new FlavorTextBestiaryInfoElement("Found only in the deepest depths of Spirit-touched Caverns, these malevolent wraiths hold one simple goal: Purloin the souls of living things."),
			});
		}

		public override void AI()
		{
			timer++;

			if (timer == 20)
			{
				NPC.noTileCollide = true;
				timer = 0;
			}

			Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2f)) / 16f), (int)((NPC.position.Y + (NPC.height / 2f)) / 16f), 0f, 0.0675f, 0.250f);

			if (Main.rand.NextBool(150))
			{
				Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
				direction.Normalize();
				direction.X *= 6f;
				direction.Y *= 6f;

				int amountOfProjectiles = Main.rand.Next(1, 2);
				for (int i = 0; i < amountOfProjectiles; ++i)
				{
					float A = Main.rand.Next(-1, 1) * 0.01f;
					float B = Main.rand.Next(-1, 1) * 0.01f;
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, direction.X + A, direction.Y + B, ModContent.ProjectileType<SpiritScythe>(), 30, 1, Main.myPlayer, 0, 0);
				}
			}
			NPC.spriteDirection = NPC.direction;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Asphalt, 0f, 0f, 100, default, 1f);

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 13);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 12);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 11);
				NPC.position.X = NPC.position.X + (NPC.width / 2);
				NPC.position.Y = NPC.position.Y + (NPC.height / 2);
				NPC.width = 30;
				NPC.height = 30;
				NPC.position.X = NPC.position.X - (NPC.width / 2);
				NPC.position.Y = NPC.position.Y - (NPC.height / 2);
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
				int[] spawnTiles = new[] { ModContent.TileType<SpiritDirt>(), ModContent.TileType<SpiritStone>(), ModContent.TileType<SpiritGrass>(), ModContent.TileType<SpiritIce>() };
				return spawnTiles.Contains(spawnInfo.SpawnTileType) ? 2f : 0f;
			}
			return 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddCommon(ModContent.ItemType<SoulShred>(), 3);
	}
}