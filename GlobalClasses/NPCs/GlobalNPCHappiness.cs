using Microsoft.Xna.Framework;
using SpiritMod.NPCs.Town;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritMod.GlobalClasses.NPCs
{
	internal class GlobalNPCHappiness : GlobalNPC
	{
		public override void SetStaticDefaults()
		{
			NPCHappiness.Get(NPCID.Wizard)
				.SetNPCAffection<Gambler>(AffectionLevel.Like)
				.SetNPCAffection<RuneWizard>(AffectionLevel.Like);
			NPCHappiness.Get(NPCID.Pirate)
				.SetNPCAffection<Adventurer>(AffectionLevel.Like)
				.SetNPCAffection<Gambler>(AffectionLevel.Like)
				.SetNPCAffection<Rogue>(AffectionLevel.Dislike);
			NPCHappiness.Get(NPCID.GoblinTinkerer)
				.SetNPCAffection<RuneWizard>(AffectionLevel.Like)
				.SetNPCAffection<Gambler>(AffectionLevel.Dislike);
			NPCHappiness.Get(NPCID.ArmsDealer)
				.SetNPCAffection<Adventurer>(AffectionLevel.Like)
				.SetNPCAffection<Rogue>(AffectionLevel.Hate);
			NPCHappiness.Get(NPCID.Demolitionist)
				.SetNPCAffection<RuneWizard>(AffectionLevel.Like)
				.SetNPCAffection<Rogue>(AffectionLevel.Like);
			NPCHappiness.Get(NPCID.DD2Bartender).SetNPCAffection<Adventurer>(AffectionLevel.Like);
			NPCHappiness.Get(NPCID.BestiaryGirl).SetNPCAffection<Adventurer>(AffectionLevel.Like);
		}

		public override void GetChat(NPC npc, ref string chat)
		{
			float replaceChance = 0;
			List<string> dialogue = new();

			if (npc.type == NPCID.Wizard)
			{
				AddDialogueAboutNPC<RuneWizard>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Wizard.RuneWizard"), 0.3f, npc, ref replaceChance);
				AddDialogueAboutNPC<Gambler>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Wizard.Gambler1"), 0.1f, npc, ref replaceChance);
				AddDialogueAboutNPC<Gambler>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Wizard.Gambler2"), 0.1f, npc, ref replaceChance);
			}
			else if (npc.type == NPCID.Pirate)
			{
				AddDialogueAboutNPC<Adventurer>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Pirate.Adventurer"), 0.2f, npc, ref replaceChance);
				AddDialogueAboutNPC<Rogue>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Pirate.Rogue"), 0.3f, npc, ref replaceChance);
				AddDialogueAboutNPC<Gambler>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Pirate.Gambler"), 0.3f, npc, ref replaceChance);
			}
			else if (npc.type == NPCID.GoblinTinkerer)
			{
				AddDialogueAboutNPC<RuneWizard>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.GoblinTinkerer.RuneWizard"), 0.4f, npc, ref replaceChance);
				AddDialogueAboutNPC<Gambler>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.GoblinTinkerer.Gambler"), 0.2f, npc, ref replaceChance);
			}
			else if (npc.type == NPCID.ArmsDealer)
			{
				AddDialogueAboutNPC<Rogue>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.ArmsDealer.Rogue1"), 0.3f, npc, ref replaceChance);
				AddDialogueAboutNPC<Rogue>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.ArmsDealer.Rogue2"), 0.3f, npc, ref replaceChance);
				AddDialogueAboutNPC<Adventurer>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.ArmsDealer.Adventurer"), 0.2f, npc, ref replaceChance);
			}
			else if (npc.type == NPCID.Demolitionist)
			{
				AddDialogueAboutNPC<RuneWizard>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Demolitionist.RuneWizard"), 0.4f, npc, ref replaceChance);
				AddDialogueAboutNPC<Rogue>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Demolitionist.Rogue"), 0.2f, npc, ref replaceChance);
			}
			else if (npc.type == NPCID.Golfer)
				AddDialogueAboutNPC<Gambler>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Golfer.Gambler"), 0.3f, npc, ref replaceChance);
			else if (npc.type == NPCID.DD2Bartender)
				AddDialogueAboutNPC<Adventurer>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.DD2Bartender.Adventurer"), 0.2f, npc, ref replaceChance);
			else if (npc.type == NPCID.Nurse)
				AddDialogueAboutNPC<Gambler>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.Nurse.Gambler"), 0.2f, npc, ref replaceChance);
			else if (npc.type == NPCID.BestiaryGirl && !Main.bloodMoon && Main.GetMoonPhase() != Terraria.Enums.MoonPhase.Full)
				AddDialogueAboutNPC<Adventurer>(dialogue, Language.GetTextValue("Mods.SpiritMod.TownNPC.Vanilla.BestiaryGirl.Adventurer"), 0.2f, npc, ref replaceChance);

			if (Main.rand.NextFloat() < replaceChance)
				chat = Main.rand.Next(dialogue);
		}

		public static void AddDialogueAboutNPC<T>(List<string> dialogue, string text, float increase, NPC npc, ref float replaceChance) where T : ModNPC => AddDialogueAboutNPC(dialogue, ModContent.NPCType<T>(), text, increase, npc, ref replaceChance);
		public static void AddDialogueAboutNPC(List<string> dialogue, int npcID, string text, float increase, NPC self, ref float replaceChance)
		{
			int npc = NPC.FindFirstNPC(npcID);
			if (npc >= 0 && IsTownNPCNearby(self, npcID, out bool _))
			{
				dialogue.Add(text.Replace("{N}", $"{Main.npc[npc].GivenName}"));

				static float CombineChances(float p1, float p2) => p1 + p2 - (p1 * p2);

				replaceChance = CombineChances(replaceChance, increase);
			}
		}

		/// <summary>Adapted from vanilla's ShopHelper.</summary>
		internal static List<NPC> GetNearbyResidentNPCs(NPC current, out int npcsWithinHouse, out int npcsWithinVillage)
		{
			List<NPC> list = new List<NPC>();
			npcsWithinHouse = 0;
			npcsWithinVillage = 0;

			Vector2 npcHome = new(current.homeTileX, current.homeTileY);
			if (current.homeless)
				npcHome = new(current.Center.X / 16f, current.Center.Y / 16f);

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (i == current.whoAmI)
					continue;

				NPC npc = Main.npc[i];
				if (npc.active && npc.townNPC && !IsNotReallyTownNPC(npc) && !WorldGen.TownManager.CanNPCsLiveWithEachOther_ShopHelper(current, npc))
				{
					Vector2 otherHome = new(npc.homeTileX, npc.homeTileY);
					if (npc.homeless)
						otherHome = npc.Center / 16f;

					float dist = Vector2.DistanceSquared(npcHome, otherHome);

					if (dist < 25f * 25f)
					{
						list.Add(npc);
						npcsWithinHouse++;
					}
					else if (dist < 120f * 120f)
						npcsWithinVillage++;
				}
			}
			return list;
		}

		/// <summary>Adapted from vanilla's ShopHelper.</summary>
		internal static bool IsTownNPCNearby(NPC current, int otherNPCType, out bool otherNPCInHome)
		{
			otherNPCInHome = false;
			Vector2 npcHome = new(current.homeTileX, current.homeTileY);
			if (current.homeless)
				npcHome = new(current.Center.X / 16f, current.Center.Y / 16f);

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (i == current.whoAmI)
					continue;

				NPC npc = Main.npc[i];
				if (npc.active && npc.townNPC && !IsNotReallyTownNPC(npc) && !WorldGen.TownManager.CanNPCsLiveWithEachOther_ShopHelper(current, npc) && npc.type == otherNPCType)
				{
					Vector2 otherHome = new(npc.homeTileX, npc.homeTileY);
					if (npc.homeless)
						otherHome = npc.Center / 16f;

					float dist = Vector2.DistanceSquared(npcHome, otherHome);

					if (dist < 25f * 25f)
					{
						otherNPCInHome = true;
						return true;
					}
					else if (dist < 120f * 120f)
						return true;
				}
			}
			return false;
		}

		/// <summary>Adapted from vanilla's ShopHelper.</summary>
		private static bool IsNotReallyTownNPC(NPC npc) => npc.type == NPCID.OldMan || npc.type == NPCID.TravellingMerchant || NPCID.Sets.ActsLikeTownNPC[npc.type];
	}
}