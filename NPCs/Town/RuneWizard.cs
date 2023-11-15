using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Biomes;
using SpiritMod.Items.Glyphs;
using SpiritMod.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using SpiritMod.Items.Armor.WitchSet;

namespace SpiritMod.NPCs.Town
{
	[AutoloadHead]
	public class RuneWizard : ModNPC
	{
		public override string Texture => "SpiritMod/NPCs/Town/RuneWizard";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 26;
			NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
			NPCID.Sets.AttackFrameCount[NPC.type] = 4;
			NPCID.Sets.DangerDetectRange[NPC.type] = 1500;
			NPCID.Sets.AttackType[NPC.type] = 0;
			NPCID.Sets.AttackTime[NPC.type] = 25;
			NPCID.Sets.AttackAverageChance[NPC.type] = 30;

			NPC.Happiness
				.SetBiomeAffection<SpiritSurfaceBiome>(AffectionLevel.Like).SetBiomeAffection<SpiritUndergroundBiome>(AffectionLevel.Like)
				.SetBiomeAffection<UndergroundBiome>(AffectionLevel.Like)
				.SetBiomeAffection<HallowBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Love)
				.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Like)
				.SetNPCAffection(NPCID.Wizard, AffectionLevel.Hate);
		}

		public override void SetDefaults()
		{
			NPC.CloneDefaults(NPCID.Guide);
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.aiStyle = 7;
			NPC.damage = 14;
			NPC.defense = 30;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.4f;
			AnimationType = NPCID.Guide;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<SpiritSurfaceBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		public override bool CanTownNPCSpawn(int numTownNPCs) => Main.player.Any(x => x.active && x.inventory.Any(y => y.type == ModContent.ItemType<Glyph>()));

		public override List<string> SetNPCNameList()
		{
			List<string> nameList = new();
			for (int i = 1; i < 9; i++)
				nameList.Add(Language.GetTextValue("Mods.SpiritMod.TownNPC.RuneWizard.Name" + i));

			return nameList;
		}

		public override string GetChat()
		{
			List<string> dialogue = new();
			for (int i = 1; i < 8; i++)
				dialogue.Add(Language.GetTextValue("Mods.SpiritMod.TownNPC.RuneWizard.Dialogue.Basic" + i));

			dialogue.AddWithCondition(Language.GetTextValue("Mods.SpiritMod.TownNPC.RuneWizard.Dialogue.Special1"), MyWorld.blueMoon);
			dialogue.AddWithCondition(Language.GetTextValue("Mods.SpiritMod.TownNPC.RuneWizard.Dialogue.Special2"), Main.hardMode);

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
			shop.Add<NullGlyph>();
			shop.Add<WitchHead>(Condition.TimeNight);
			shop.Add<WitchBody>(Condition.TimeNight);
			shop.Add<WitchLegs>(Condition.TimeNight);

			void CustomWare<T>(int price = 1, params Condition[] conditions) where T : ModItem
			{
				shop.Add(new Item(ModContent.ItemType<T>())
				{
					shopCustomPrice = price,
					shopSpecialCurrency = SpiritMod.GlyphCurrencyID
				}, conditions);
			}

			CustomWare<FrostGlyph>();
			CustomWare<RageGlyph>();
			CustomWare<RadiantGlyph>(1, Condition.DownedEyeOfCthulhu);
			CustomWare<SanguineGlyph>(3, Condition.BloodMoonOrHardmode);
			CustomWare<StormGlyph>(2, SpiritConditions.VinewrathDown);
			CustomWare<UnholyGlyph>(2, Condition.DownedEowOrBoc);
			CustomWare<VeilGlyph>(3, Condition.DownedSkeletron);
			CustomWare<BeeGlyph>(3, Condition.DownedQueenBee);
			CustomWare<BlazeGlyph>(3, Condition.Hardmode);
			CustomWare<PhaseGlyph>(3, Condition.Hardmode);
			CustomWare<VoidGlyph>(4, Condition.DownedMechBossAll);

			shop.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 18;
			knockback = 3f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 5;
			randExtraCooldown = 5;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.RubyBolt;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 14f;
			randomOffset = 2f;
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life > 0 || Main.netMode == NetmodeID.Server)
				return;

			for (int i = 0; i < 4; i++)
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Enchanter" + (i + 1)).Type);
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

		public override ITownNPCProfile TownNPCProfile() => new RuneWizardProfile();
	}

	public class RuneWizardProfile : ITownNPCProfile
	{
		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public ReLogic.Content.Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.altTexture == 1 && !(npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn))
				return ModContent.Request<Texture2D>("SpiritMod/NPCs/Town/RuneWizard_Alt_1");

			return ModContent.Request<Texture2D>("SpiritMod/NPCs/Town/RuneWizard");
		}

		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("SpiritMod/NPCs/Town/RuneWizard_Head");
	}
}