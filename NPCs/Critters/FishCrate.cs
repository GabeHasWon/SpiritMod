using SpiritMod.Items.Armor.DiverSet;
using SpiritMod.Items.Consumable.Fish;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.Critters
{
	public class FishCrate : ModNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => false;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Packing Crate");
			Main.npcFrameCount[NPC.type] = 1;
			Main.npcCatchable[NPC.type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 40;
			NPC.damage = 0;
			NPC.knockBackResist = 0f;
			NPC.defense = 1000;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.lifeMax = 10;
			NPC.aiStyle = 1;
			NPC.npcSlots = 0;
			NPC.catchItem = (short)ModContent.ItemType<Items.Placeable.FishCrate>();
			NPC.noGravity = false;
			NPC.alpha = 40;
			NPC.dontCountMe = true;

			AIType = NPCID.Grasshopper;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.UIInfoProvider = new CritterUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type]);

			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("Stray cargo washed ashore from who knows where. Nobody seems to be coming for it, best to net it out of the water. Finders keepers after all."),
			});
		}

		public override void AI()
		{
			NPC.spriteDirection = -NPC.direction;

			if (NPC.wet)
			{
				NPC.aiStyle = 1;
				NPC.npcSlots = 0;
				NPC.noGravity = false;
				AIType = NPCID.Grasshopper;
				NPC.velocity.X *= 0f;
				NPC.velocity.Y *= .9f;
			}
			else
			{
				NPC.aiStyle = 0;
				NPC.npcSlots = 0;
				NPC.noGravity = false;
				AIType = NPCID.BoundGoblin;
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life > 0 || Main.netMode == NetmodeID.Server)
				return;

			for (int i = 0; i < 5; i++)
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("FishCrate" + (i + 1)).Type, 1f);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<RawFish>(2);
			npcLoot.AddCommon(ItemID.GoldCoin, 5, 20);
			npcLoot.AddOneFromOptions(27, ItemID.ReaverShark, ItemID.Swordfish, ItemID.SawtoothShark);
			npcLoot.AddOneFromOptions(3, ItemID.FrostDaggerfish, ItemID.BombFish);
			npcLoot.AddOneFromOptions(4, ItemID.ArmoredCavefish, ItemID.Damselfish, ItemID.DoubleCod, ItemID.FrostMinnow);
			npcLoot.AddOneFromOptions(1, ItemID.Shrimp, ItemID.Salmon, ItemID.Bass, ItemID.RedSnapper, ItemID.Trout);
			npcLoot.AddOneFromOptions(10, ItemID.FlarefinKoi, ItemID.Obsidifish, ItemID.Prismite, ItemID.PrincessFish);
			npcLoot.AddOneFromOptions<DiverLegs, DiverHead, DiverBody>(14);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.OceanMonster.Chance * 0.05f;
	}
}