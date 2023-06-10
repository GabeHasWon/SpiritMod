using Microsoft.Xna.Framework;
using SpiritMod.Items.Sets.SpiritBiomeDrops;
using SpiritMod.Tiles.Block;
using SpiritMod.Utilities;
using System.Linq;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace SpiritMod.NPCs.Spirit
{
	public class HauntedBook : ModNPC
	{
		private int Counter
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ancient Tome");
			Main.npcFrameCount[NPC.type] = 6;
		}

		public override void SetDefaults()
		{
			NPC.width = 48;
			NPC.height = 40;
			NPC.damage = 45;
			NPC.defense = 12;
			NPC.lifeMax = 410;
			NPC.HitSound = SoundID.NPCHit3;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 3060f;
			NPC.knockBackResist = .45f;
			NPC.aiStyle = -1;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiritUndergroundBiome>().Type };

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.SpiritTomeBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				new FlavorTextBestiaryInfoElement("A mysterious tome possessed by ancient spirits. Each contains a strange set of pages written in an indecipherable language."),
			});
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter = (NPC.frameCounter + 0.15f) % Main.npcFrameCount[NPC.type];
			NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;

            if (!player.ZoneSpirit())
                return 0f;

            if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust) && 
				((!Main.pumpkinMoon && !Main.snowMoon) || spawnInfo.SpawnTileY > Main.worldSurface || Main.dayTime) && (!Main.eclipse || spawnInfo.SpawnTileY > Main.worldSurface || !Main.dayTime) && (SpawnCondition.GoblinArmy.Chance == 0)) {
				int[] TileArray2 = { ModContent.TileType<SpiritDirt>(), ModContent.TileType<SpiritStone>(), ModContent.TileType<Spiritsand>(), ModContent.TileType<SpiritGrass>(), ModContent.TileType<SpiritIce>(), };
				return TileArray2.Contains(spawnInfo.SpawnTileType) && NPC.downedMechBossAny ? 2.09f : 0f;
			}
			return 0f;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
				for (int i = 11; i < 14; ++i)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, i);
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
			Player target = Main.player[NPC.target];

			if (Main.rand.NextBool(150))
			{
				int numShots = Main.rand.Next(2, 3);

				for (int i = 0; i < numShots; ++i)
				{
					Vector2 direction = (NPC.Center.DirectionTo(target.Center) * 2f).RotatedByRandom(0.05f);
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction, ModContent.ProjectileType<RuneHostile>(), 38, 1, Main.myPlayer);
				}
			}

			Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 0.675f, 2.50f);

			if (++Counter % 25 == 0)
			{
				Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
				direction.Normalize();
				NPC.velocity.Y = direction.Y * 3f;
				NPC.velocity.X = direction.X * 3f;
			}
			NPC.spriteDirection = NPC.direction;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon(ModContent.ItemType<Items.Sets.RunicSet.Rune>(), 2, 1, 2);
			npcLoot.AddCommon(ModContent.ItemType<PossessedBook>(), 20);
		}
	}
}
