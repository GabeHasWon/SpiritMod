using Microsoft.Xna.Framework;
using SpiritMod.Biomes;
using SpiritMod.Items.Sets.HuskstalkSet;
using SpiritMod.Utilities;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace SpiritMod.NPCs.Reach
{
	public class ReachSlime : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Briarthorn Slime");
			Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
			NPCHelper.ImmuneTo(this, BuffID.Poisoned, BuffID.Venom);
		}

		public override void SetDefaults()
		{
			NPC.width = 64;
			NPC.height = 24;
			NPC.damage = 12;
			NPC.defense = 4;
			NPC.lifeMax = 46;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 30f;
			NPC.alpha = 60;
			NPC.knockBackResist = .25f;
			NPC.aiStyle = 1;
			AIType = NPCID.BlueSlime;
			AnimationType = NPCID.BlueSlime;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BriarthornSlimeBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<BriarSurfaceBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		public override void AI()
		{
			float targetScale = (.2f * (NPC.life / (float)NPC.lifeMax)) + .8f;

			NPC.scale = MathHelper.Lerp(NPC.scale, targetScale, 0.2f);
			NPC.spriteDirection = -NPC.direction;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;

			return (spawnInfo.Player.ZoneBriar() && Main.dayTime && !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust) && 
				((!Main.pumpkinMoon && !Main.snowMoon) || spawnInfo.SpawnTileY > Main.worldSurface || Main.dayTime) && 
				(!Main.eclipse || spawnInfo.SpawnTileY > Main.worldSurface || !Main.dayTime) && !spawnInfo.Invasion && !spawnInfo.PlayerInTown && SpawnCondition.GoblinArmy.Chance == 0) ? 2.1f : 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon(ItemID.Gel, 1, 1, 3);
			npcLoot.AddCommon<AncientBark>(2, 1, 3);
			npcLoot.AddCommon(ItemID.SlimeStaff, 10000);
			npcLoot.AddCommon(ItemID.Bezoar, 200);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			if (Main.rand.NextBool(3))
				target.AddBuff(BuffID.Poisoned, 180);
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 12; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SlimeBunny, 2.5f * hit.HitDirection, -2.5f, 0, Color.Green, 0.7f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SlimeBunny, 2.5f * hit.HitDirection, -2.5f, 0, Color.Green, 0.7f);
			}
		}
	}
}