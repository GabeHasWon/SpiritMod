using Microsoft.Xna.Framework;
using SpiritMod.Items.Equipment;
using SpiritMod.Items.Sets.SpiritBiomeDrops;
using SpiritMod.Tiles.Block;
using SpiritMod.Utilities;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace SpiritMod.NPCs.Spirit;

public class HauntedBook : ModNPC
{
	private ref float Timer => ref NPC.ai[0];

	public override void SetStaticDefaults() => Main.npcFrameCount[NPC.type] = 6;

	public override void SetDefaults()
	{
		NPC.width = 48;
		NPC.height = 40;
		NPC.damage = 0;
		NPC.defense = 12;
		NPC.lifeMax = 410;
		NPC.HitSound = SoundID.NPCHit3;
		NPC.DeathSound = SoundID.NPCDeath6;
		NPC.value = 3060f;
		NPC.knockBackResist = .45f;
		NPC.aiStyle = -1;
		NPC.noTileCollide = true;
		NPC.noGravity = true;
		SpawnModBiomes = [ModContent.GetInstance<Biomes.SpiritUndergroundBiome>().Type];

		Banner = NPC.type;
		BannerItem = ModContent.ItemType<Items.Banners.SpiritTomeBanner>();
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

	public override void FindFrame(int frameHeight)
	{
		float speed = 0.15f * (1 - NPC.life / NPC.lifeMax) + 0.15f;
		NPC.frameCounter = (NPC.frameCounter + speed) % Main.npcFrameCount[NPC.type];
		NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		Player player = spawnInfo.Player;

        if (player.ZoneSpirit() && !spawnInfo.PlayerSafe && !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust) && (!(Main.pumpkinMoon || Main.snowMoon || Main.eclipse) || spawnInfo.SpawnTileY > Main.worldSurface) && SpawnCondition.GoblinArmy.Chance == 0)
		{
			int[] spawnTiles = { ModContent.TileType<SpiritDirt>(), ModContent.TileType<SpiritStone>(), ModContent.TileType<Spiritsand>(), ModContent.TileType<SpiritGrass>(), ModContent.TileType<SpiritIce>() };
			return spawnTiles.Contains(spawnInfo.SpawnTileType) ? (spawnInfo.Player.Center.Y / 16 < Main.worldSurface ? 1f : 2f) : 0f;
		}

		return 0f;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			for (int i = 11; i < 14; ++i)
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, i);
	}

	public override void AI()
	{
		NPC.TargetClosest(true);
		Timer++;

		Player target = Main.player[NPC.target];
		bool nearTarget = target.DistanceSQ(NPC.Center) < 240 * 240;

		if (nearTarget)
			Timer += 2;

		if (Timer > 550 || nearTarget)
			NPC.velocity *= 0.98f;
		else if (Timer > 25 && !nearTarget)
			NPC.velocity += Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 0.4f;

		if (NPC.velocity.LengthSquared() > 5 * 5)
			NPC.velocity = Vector2.Normalize(NPC.velocity) * 5;

		if (Timer > 600 && Main.netMode != NetmodeID.MultiplayerClient)
		{
			Vector2 direction = (NPC.Center.DirectionTo(target.Center) * 2f).RotatedByRandom(0.05f) * Main.rand.NextFloat(0.8f, 1.2f);
			int damage = NPCUtils.ToActualDamage(40, 1.5f, 1.75f);
			Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction, ModContent.ProjectileType<RuneHostile>(), damage, 1, Main.myPlayer);

			Timer = 0;
			NPC.velocity -= direction * 4;

			SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, NPC.Center);
		}

		Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 0.675f, 2.50f);

		NPC.spriteDirection = NPC.direction;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.AddCommon(ModContent.ItemType<Items.Sets.RunicSet.Rune>(), 2, 1, 2);
		npcLoot.AddCommon(ModContent.ItemType<PossessedBook>(), 20);
		npcLoot.AddCommon(ModContent.ItemType<Obolos>(), 100);
	}
}
