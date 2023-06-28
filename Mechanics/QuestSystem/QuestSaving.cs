﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.Mechanics.QuestSystem;

internal class QuestSaving
{
	public static void LoadData(TagCompound tag)
	{
		try
		{
			QuestManager.ActiveQuests.Clear(); //Clear active quests in order to not clog

			List<string> allQuests = tag.Get<List<string>>("SpiritMod:AllQuests");

			if (!Main.dedServ)
				SpiritMod.QuestHUD.Clear();

			QuestManager.QuestBookUnlocked = tag.Get<bool>("SpiritMod:QuestBookUnlocked");

			for (int i = 0; i < QuestManager.Quests.Count; i++)
			{
				Quest quest = QuestManager.Quests[i];
				quest.ResetEverything();

				// get the key for this quest
				string key = "SpiritMod:" + quest.QuestName;
				if (!tag.ContainsKey(key))
				{
					bool failed = true;
					foreach (string s in quest._altNames)
					{
						key = "SpiritMod:" + s;
						if (tag.ContainsKey(key))
						{
							failed = false;
							break;
						}
					}

					if (failed) // this quest doesn't exist at all, so skip.
						continue;
				}

				StoredQuestData data = ConvertBack(tag.Get<TagCompound>(key));

				quest.IsUnlocked = data.IsUnlocked;
				quest.IsCompleted = data.IsCompleted;
				quest.RewardsGiven = data.RewardsGiven;

				if (data.IsActive)
				{
					QuestManager.ActivateQuest(quest);
					quest.ReadFromDataBuffer(data.Buffer);
				}

				quest.ActiveTime = data.TimeLeftActive;
				quest.UnlockTime = data.TimeLeftUnlocked;

				if (allQuests.Contains(key))
					allQuests.Remove(key);
			}

			// get all the unloaded quests
			QuestManager.UnloadedQuests.Clear();
			for (int i = 0; i < allQuests.Count; i++)
				QuestManager.UnloadedQuests.Add(allQuests[i], ConvertBack(tag.Get<TagCompound>(allQuests[i])));

			LoadQueue(tag);
		}
		catch (Exception e)
		{
			SpiritMod.Instance.Logger.Error("Error loading quests! Error:\n" + e);
		}
	}

	public static void SaveData(TagCompound tag)
	{
		List<string> allQuestNames = new List<string>();

		tag.Add("SpiritMod:QuestBookUnlocked", QuestManager.QuestBookUnlocked);

		// save any quests necessary
		for (int i = 0; i < QuestManager.Quests.Count; i++)
		{
			Quest quest = QuestManager.Quests[i];
			var data = ToStoredQuest(quest);

			allQuestNames.Add("SpiritMod:" + quest.QuestName);
			tag.Add(allQuestNames[i], Convert(data));
		}

		// add unloaded quests so their data is saved
		foreach (var pair in QuestManager.UnloadedQuests)
		{
			allQuestNames.Add(pair.Key);
			tag.Add(pair.Key, Convert(pair.Value));
		}

		tag.Add("SpiritMod:AllQuests", allQuestNames);

		SaveQueue(tag);
	}

	private static void SaveQueue(TagCompound tag)
	{
		var queue = ModContent.GetInstance<QuestWorld>().NPCQuestQueue;

		tag.Add("SpiritMod:QuestQueueNPCLength", queue.Keys.Count);

		int npcIDRep = 0;
		int questRep = 0;
		foreach (int item in queue.Keys)
		{
			tag.Add("SpiritMod:QuestQueueNPCID" + npcIDRep, item); //Writes the ID and the length of the queue
			tag.Add("SpiritMod:SingleQuestQueueLength" + npcIDRep++, queue[item].Count);

			List<Quest> requeued = new();

			while (queue[item].Count > 0) //Writes every value in the queue
			{
				Quest q = queue[item].Dequeue();
				tag.Add("SpiritMod:SingleQuestQueue" + questRep++, q.QuestName);

				requeued.Add(q);
			}

			foreach (var quest in requeued)
				queue[item].Enqueue(quest);
		}
	}

	private static void LoadQueue(TagCompound tag)
	{
		int queuesCount = tag.GetInt("SpiritMod:QuestQueueNPCLength");

		if (queuesCount == 0)
			return; //Nothing in the dictionary, exit

		int questCount = 0;
		for (int i = 0; i < queuesCount; ++i)
		{
			int npcID = tag.GetInt("SpiritMod:QuestQueueNPCID" + i);
			int length = tag.GetInt("SpiritMod:SingleQuestQueueLength" + i);

			if (length == 0)
				continue; //Nothing in the queue, skip

			for (int j = 0; j < length; ++j)
			{
				string questName = tag.GetString("SpiritMod:SingleQuestQueue" + questCount++);
				var quest = QuestManager.Quests.FirstOrDefault(x => x.QuestName == questName);

				if (quest != null)
					QuestWorld.AddToQueue(npcID, quest);
			}
		}
	}

	public static StoredQuestData ToStoredQuest(Quest quest)
	{
		var data = new StoredQuestData
		{
			IsActive = quest.IsActive,
			IsUnlocked = quest.IsUnlocked,
			IsCompleted = quest.IsCompleted,
			RewardsGiven = quest.RewardsGiven,
			TimeLeftActive = (short)quest.ActiveTime,
			TimeLeftUnlocked = (short)quest.UnlockTime
		};

		if (quest.IsActive)
		{
			byte[] buffer = quest.GetTaskDataBuffer();
			data.Buffer = buffer;
		}
		return data;
	}

	private static TagCompound Convert(StoredQuestData data)
	{
		var tag = new TagCompound
			{
				{ "a", data.IsActive },
				{ "u", data.IsUnlocked },
				{ "c", data.IsCompleted },
				{ "r", data.RewardsGiven },
				{ "tlu", data.TimeLeftUnlocked },
				{ "tla", data.TimeLeftActive },
				{ "b", data.Buffer }
			};
		return tag;
	}

	private static StoredQuestData ConvertBack(TagCompound tag)
	{
		var data = new StoredQuestData
		{
			IsActive = tag.Get<bool>("a"),
			IsUnlocked = tag.Get<bool>("u"),
			IsCompleted = tag.Get<bool>("c"),
			RewardsGiven = tag.Get<bool>("r"),
			TimeLeftUnlocked = tag.Get<short>("tlu"),
			TimeLeftActive = tag.Get<short>("tla"),
			Buffer = tag.Get<byte[]>("b")
		};
		return data;
	}
}