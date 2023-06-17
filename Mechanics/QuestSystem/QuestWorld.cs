using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Quests;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using System.Linq;

namespace SpiritMod.Mechanics.QuestSystem
{
	public class QuestWorld : ModSystem
	{
		public Dictionary<int, Queue<Quest>> NPCQuestQueue { get; private set; } = new Dictionary<int, Queue<Quest>>();

		public override void PostUpdateWorld()
		{
			if (!QuestManager.QuestBookUnlocked) //Do nothing if we don't have the book
				return;

			if (Main.hardMode)
			{
				QuestManager.UnlockQuest<ExplorerQuestBlueMoon>(true);
				QuestManager.UnlockQuest<SlayerQuestVultureMatriarch>(true);

				if (Main.bloodMoon && QuestManager.GetQuest<SlayerQuestClown>().IsUnlocked)
					AddQuestQueue(NPCID.PartyGirl, QuestManager.GetQuest<SlayerQuestClown>());

				AddQuestQueue(NPCID.Dryad, QuestManager.GetQuest<OlympiumQuest>());
				AddQuestQueue(NPCID.Mechanic, QuestManager.GetQuest<GranitechQuest>());
				AddQuestQueue(ModContent.NPCType<NPCs.Town.Adventurer>(), QuestManager.GetQuest<AuroraStagQuest>());

				if (NPC.downedMechBoss1 || NPC.downedMechBoss2 || NPC.downedMechBoss3)
					AddQuestQueue(NPCID.Dryad, QuestManager.GetQuest<CritterCaptureSoulOrb>());
			}

			//if (NPC.downedBoss2)
			//	AddQuestQueue(NPCID.Stylist, QuestManager.GetQuest<StylistQuestMeteor>());

#if DEBUG
			if (Main.mouseLeft && Main.mouseRight && Main.LocalPlayer.controlJump && Main.LocalPlayer.controlHook)
				QuestManager.UnlockAll();

			if (Main.mouseLeft && Main.mouseRight && Main.LocalPlayer.controlJump && Main.LocalPlayer.controlMount)
			{
				QuestManager.UnlockAll();
				QuestManager.CompleteAll();
			}
#endif
		}

		/// <summary>
		/// Adds a quest to a specific NPC's queue. Use QuestManager.GetQuest<Quest>() for the parameter.
		/// </summary>
		/// <param name="npcID">The ID of the NPC that will recieve a new quest.</param>
		/// <param name="quest">The quest to add to the queue. Use QuestManager.GetQuest<Quest>() for this.</param>
		public void AddQuestQueue(int npcID, Quest quest)
		{
			if (quest is null)
				return;

			if (!NPCQuestQueue.ContainsKey(npcID))
				NPCQuestQueue.Add(npcID, new Queue<Quest>());

			if (!NPCQuestQueue[npcID].Contains(quest) && !QuestManager.GetQuest(quest).IsUnlocked)
				NPCQuestQueue[npcID].Enqueue(quest);
		}

		/// <summary>
		/// Static mirror to <see cref="AddQuestQueue(int, Quest)"/>. Works the same, just easier to write.
		/// </summary>
		public static void AddToQueue(int npcID, Quest quest) => ModContent.GetInstance<QuestWorld>().AddQuestQueue(npcID, quest);

		/// <summary>
		/// Used to load pre-1.4.4.14 Quests so people don't have to restart.
		/// </summary>
		/// <param name="tag"></param>
		public override void LoadWorldData(TagCompound tag)
		{
			if (!QuestManager.QuestBookUnlocked) //Try and load the questbook from legacy if it's not already loaded
				QuestSaving.LoadData(tag);
		}
	}
}