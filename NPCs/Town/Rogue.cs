using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SpiritMod.Biomes;
using SpiritMod.Items.Accessory;
using SpiritMod.Items.Armor;
using SpiritMod.Items.Weapon.Thrown;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using SpiritMod.Items.Weapon.Thrown.PlagueVial;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Items.Armor.Masks;

namespace SpiritMod.NPCs.Town
{
	[AutoloadHead]
	public class Rogue : ModNPC
	{
		public override string Texture => "SpiritMod/NPCs/Town/Rogue";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 26;
			NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
			NPCID.Sets.AttackFrameCount[NPC.type] = 4;
			NPCID.Sets.DangerDetectRange[NPC.type] = 1500;
			NPCID.Sets.AttackType[NPC.type] = 0;
			NPCID.Sets.AttackTime[NPC.type] = 16;
			NPCID.Sets.AttackAverageChance[NPC.type] = 30;

			NPC.Happiness
				.SetBiomeAffection<BriarSurfaceBiome>(AffectionLevel.Like).SetBiomeAffection<BriarUndergroundBiome>(AffectionLevel.Like)
				.SetBiomeAffection<JungleBiome>(AffectionLevel.Like)
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Love)
				.SetNPCAffection<Adventurer>(AffectionLevel.Like)
				.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Hate);
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
			NPC.knockBackResist = 0.5f;
			AnimationType = NPCID.Guide;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Jungle");

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server) {
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit1").Type);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit2").Type);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit3").Type);
			}
		}

		public override bool CanTownNPCSpawn(int numTownNPCs) => Main.player.Any(x => x.active) && !NPC.AnyNPCs(ModContent.NPCType<Rogue>()) && !NPC.AnyNPCs(ModContent.NPCType<BoundRogue>());

		public override List<string> SetNPCNameList()
		{
			List<string> nameList = new();
			for (int i = 1; i < 9; i++)
				nameList.Add(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Rogue.Name" + i));

			return nameList;
		}

		public override string GetChat()
		{
			List<string> dialogue = new();
			for (int i = 1; i < 8; i++)
				dialogue.Add(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Rogue.Dialogue.Basic" + i));

			int wizard = NPC.FindFirstNPC(NPCID.Wizard);
			if (wizard >= 0)
				dialogue.Add(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Rogue.Dialogue.Special1", Main.npc[wizard].GivenName));

			int merchant = NPC.FindFirstNPC(NPCID.Merchant);
			if (merchant >= 0)
				dialogue.Add(Language.GetTextValue("Mods.SpiritMod.TownNPCText.Rogue.Dialogue.Special2", Main.npc[merchant].GivenName));

			return Main.rand.Next(dialogue);
		}

		public override void SetChatButtons(ref string button, ref string button2) => button = Language.GetTextValue("LegacyInterface.28");

		public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
			if (firstButton)
				shopName = "Shop";
		}

		public override void AddShops()
		{
			NPCShop shop = new NPCShop(Type);
			shop.Add(ItemID.Shuriken);
			shop.Add<RogueHood>();
			shop.Add<RoguePlate>();
			shop.Add<RoguePants>();
			shop.Add<RogueCrest>();
			shop.Add<EoWDagger>(Condition.CorruptWorld, Condition.DownedEaterOfWorlds);
			shop.Add<BoCShuriken>(Condition.CrimsonWorld, Condition.DownedBrainOfCthulhu);
			shop.Add<SkeletronHand>(Condition.DownedSkeletron);
			shop.Add<PlagueVial>(Condition.Hardmode);
			shop.Add<SwiftRune>();
			shop.Add<AssassinMagazine>();
			shop.Add<TargetCan>();
			shop.Add<TargetBottle>();
			shop.Add<TreasureChest>();
			shop.Add<PsychoMask>();

			shop.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 10;
			knockback = 3f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 5;
			randExtraCooldown = 5;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ModContent.ProjectileType<Projectiles.Thrown.Kunai_Throwing>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 13f;
			randomOffset = 2f;
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

		public override ITownNPCProfile TownNPCProfile() => new RogueProfile();
	}

	public class RogueProfile : ITownNPCProfile
	{
		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.altTexture == 1 && !(npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn))
				return ModContent.Request<Texture2D>("SpiritMod/NPCs/Town/Rogue_Alt_1");

			return ModContent.Request<Texture2D>("SpiritMod/NPCs/Town/Rogue");
		}

		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("SpiritMod/NPCs/Town/Rogue_Head");
	}
}
