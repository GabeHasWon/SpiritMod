using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SpiritMod.Biomes;
using SpiritMod.Items.Accessory;
using SpiritMod.Items.Pins;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Items.Sets.HuskstalkSet;
using SpiritMod.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.GameContent.Bestiary;
using SpiritMod.Items.Accessory.Leather;
using SpiritMod.Items.Sets.GunsMisc.PolymorphGun;

namespace SpiritMod.NPCs.Town
{
	[AutoloadHead]
	public class Adventurer : ModNPC
	{
		public override string Texture => "SpiritMod/NPCs/Town/Adventurer";
		public override string Name => "Adventurer";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 26;
			NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
			NPCID.Sets.AttackFrameCount[NPC.type] = 4;
			NPCID.Sets.DangerDetectRange[NPC.type] = 500;
			NPCID.Sets.AttackType[NPC.type] = 0;
			NPCID.Sets.AttackTime[NPC.type] = 16;
			NPCID.Sets.AttackAverageChance[NPC.type] = 30;

			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
				.SetBiomeAffection<BriarSurfaceBiome>(AffectionLevel.Dislike).SetBiomeAffection<BriarUndergroundBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Pirate, AffectionLevel.Love)
				.SetNPCAffection<Rogue>(AffectionLevel.Like)
				.SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Angler, AffectionLevel.Hate);
		}

		public override void SetDefaults()
		{
			NPC.CloneDefaults(NPCID.Guide);
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.aiStyle = 7;
			NPC.damage = 30;
			NPC.defense = 30;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.4f;
			AnimationType = NPCID.Guide;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Surface");

		public override bool CanTownNPCSpawn(int numTownNPCs) => Main.player.Any(x => x.active) && !NPC.AnyNPCs(NPCType<BoundAdventurer>()) && !NPC.AnyNPCs(NPCType<Adventurer>());

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server) {
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Adventurer1").Type);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Adventurer2").Type);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Adventurer3").Type);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Adventurer4").Type);
			}
		}

		public override List<string> SetNPCNameList()
		{
			List<string> nameList = new();
			for (int i = 1; i < 10; i++)
				nameList.Add(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Adventurer.Name" + i));

			return nameList;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddCommon<AdventurerMap>();

		public override string GetChat()
		{
			List<string> dialogue = new();
			for (int i = 1; i < 8; i++)
				dialogue.Add(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Adventurer.Dialogue.Basic" + i));

			int merchant = NPC.FindFirstNPC(NPCID.Merchant);
			if (merchant >= 0)
				dialogue.Add(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Adventurer.Dialogue.Special1", Main.npc[merchant].GivenName));

			int travellingMerchant = NPC.FindFirstNPC(NPCID.TravellingMerchant);
			if (travellingMerchant >= 0)
				dialogue.Add(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Adventurer.Dialogue.Special2", Main.npc[travellingMerchant].GivenName));

			int armsDealer = NPC.FindFirstNPC(NPCID.ArmsDealer);
			if (armsDealer >= 0)
				dialogue.Add(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Adventurer.Dialogue.Special3", Main.npc[armsDealer].GivenName));

			dialogue.AddWithCondition(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Adventurer.Dialogue.Special4"), !Main.dayTime);
			dialogue.AddWithCondition(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Adventurer.Dialogue.Special5"), Main.bloodMoon);
			dialogue.AddWithCondition(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Adventurer.Dialogue.Special6"), MyWorld.gennedTower && !NPC.AnyNPCs(NPCType<Rogue>()) && NPC.AnyNPCs(NPCType<BoundRogue>()));
			dialogue.AddWithCondition(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Adventurer.Dialogue.Special7"), !MyWorld.gennedTower && !NPC.AnyNPCs(NPCType<Rogue>()) && NPC.AnyNPCs(NPCType<BoundRogue>()));
			dialogue.AddWithCondition(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Adventurer.Dialogue.Special8"), NPC.downedMechBossAny);
			
			return Main.rand.Next(dialogue);
		}

		public override void AddShops()
		{
			NPCShop shop = new NPCShop(Type);
			shop.Add(new Item(ItemID.TrapsightPotion) { shopCustomPrice = 5000 });
			shop.Add(new Item(ItemID.HunterPotion) { shopCustomPrice = 5000 });
			shop.Add(new Item(ItemID.Book) { shopCustomPrice = 200 }, Condition.DownedSkeletron);
			shop.Add<WWPainting>();
			shop.Add(new Item(ItemType<SkullStick>()) { shopCustomPrice = 1000 }, SpiritConditions.InBriar);
			shop.Add(new Item(ItemType<AncientBark>()) { shopCustomPrice = 200 }, SpiritConditions.InBriar);
			shop.Add<PinGreen>();
			shop.Add<PinYellow>();
			shop.Add<ExplorerTreads>();
			shop.Add<VitalityStone>(Condition.BloodMoon);
			shop.Add(ItemID.SpelunkerGlowstick, Condition.MoonPhases04, Condition.TimeNight);
			shop.Add(ItemID.StickyGlowstick, Condition.TimeDay);
			shop.Add(ItemID.CursedTorch, Condition.TimeNight, Condition.MoonPhases04);
			shop.Add(ItemID.UltrabrightTorch, Condition.TimeNight, Condition.MoonPhases26);
			shop.Add<PinScarab>(SpiritConditions.ScarabDown);
			shop.Add<PinMoonjelly>(SpiritConditions.MJWDown);
			shop.Add<PinTree>(Condition.Hardmode);
			shop.Add<PolymorphGun>(Condition.DownedMechBossAny);
			shop.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 13;
			knockback = 3f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 5;
			randExtraCooldown = 5;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = 507;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 11f;
			randomOffset = 2f;
		}

		public override ITownNPCProfile TownNPCProfile() => new AdventurerProfile();

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");

			if (!Mechanics.QuestSystem.QuestManager.QuestBookUnlocked)
				button2 = Language.GetTextValue("Mods.SpiritMod.Quests.QuestBook");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
			if (firstButton)
				shopName = "Shop";
			else
			{
				Mechanics.QuestSystem.QuestManager.UnlockQuestBook();

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Quest, 4);
					packet.Write((byte)QuestMessageType.ObtainQuestBook);
					packet.Write(true);
					packet.Write((byte)Main.myPlayer);
					packet.Send();
				}
			}
		}

		private float animCounter;
		public override void FindFrame(int frameHeight)
		{
			if (!NPC.IsABestiaryIconDummy)
				return;

			animCounter += 0.25f;
			if (animCounter >= 16)
				animCounter = 2;
			else if (animCounter < 2)
				animCounter = 2;

			int frame = (int)animCounter;
			NPC.frame.Y = frame * frameHeight;
		}
	}

	public class AdventurerProfile : ITownNPCProfile
	{
		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.altTexture == 1 && !(npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn))
				return Request<Texture2D>("SpiritMod/NPCs/Town/Adventurer_Alt_1");

			return Request<Texture2D>("SpiritMod/NPCs/Town/Adventurer");
		}

		public int GetHeadTextureIndex(NPC npc) => GetModHeadSlot("SpiritMod/NPCs/Town/Adventurer_Head");
	}
}
