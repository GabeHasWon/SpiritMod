using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Quests;
using Terraria.ID;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader.IO;

namespace SpiritMod.Mechanics.QuestSystem
{
	public class QuestPlayer : ModPlayer
	{
		internal Dictionary<Guid, List<string>> QuestRewardsPerQuestPerWorld = new();

		public void SetRewardsForQuest(string quest)
		{
			var uID = Main.ActiveWorldFileData.UniqueId;

			if (!QuestRewardsPerQuestPerWorld.ContainsKey(uID))
				QuestRewardsPerQuestPerWorld.Add(uID, new List<string>());

			QuestRewardsPerQuestPerWorld[uID].Add(quest);
		}

		public bool HasRewardsForQuest(string quest)
		{
			var uID = Main.ActiveWorldFileData.UniqueId;
			return QuestRewardsPerQuestPerWorld.ContainsKey(uID) && QuestRewardsPerQuestPerWorld[uID].Contains(quest);
		}

		public override void SaveData(TagCompound tag)
		{
			tag.Add("SpiritMod:QuestRewardsCount", QuestRewardsPerQuestPerWorld.Count);

			int count = 0;

			foreach (var (key, value) in QuestRewardsPerQuestPerWorld)
			{
				tag.Add("SpiritMod:QuestRewardKey" + count, key.ToByteArray());
				tag.Add("SpiritMod:QuestRewardList" + count, value);
				count++;
			}
		}

		public override void LoadData(TagCompound tag)
		{
			if (tag.ContainsKey("SpiritMod:QuestRewardsCount"))
			{
				int count = tag.GetInt("SpiritMod:QuestRewardsCount");

				for (int i = 0; i < count; ++i)
				{
					byte[] gUIDKeyArr = tag.GetByteArray("SpiritMod:QuestRewardKey" + i);
					Guid key = new(gUIDKeyArr);

					QuestRewardsPerQuestPerWorld.Add(key, new List<string>(tag.GetList<string>("SpiritMod:QuestRewardList" + i)));
				}
			}
		}

		public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
		{
			if (Player.ZoneJungle && QuestManager.GetQuest<ItsNoSalmon>().IsActive && Main.rand.NextBool(10) && !Player.HasItem(ModContent.ItemType<Items.Consumable.Quest.HornetfishQuest>()))
				itemDrop = ModContent.ItemType<Items.Consumable.Quest.HornetfishQuest>();
		}

		public override void OnEnterWorld()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.RequestQuestManager, 1);
				packet.Write((byte)Player.whoAmI);
				packet.Send();
			}
		}

		/// <summary>Handles syncing the QuestManager from server.</summary>
		internal static void RecieveManager(BinaryReader reader)
		{
			QuestManager.QuestBookUnlocked = reader.ReadBoolean();
			int questCount = reader.ReadInt16();

			if (questCount != QuestManager.Quests.Count)
				throw new Exception("Inconsistent quest sizes. Network error?");

			var datas = new List<Tuple<StoredQuestData, string>>();

			for (int i = 0; i < questCount; ++i)
			{
				string name = reader.ReadString();
				long flags = reader.ReadInt64();

				var data = QuestMultiplayer.ReadQuestFromLong(flags);

				byte bufferLength = reader.ReadByte();
				byte[] buffer = null;
				if (bufferLength > 0)
				{
					buffer = new byte[bufferLength];
					for (int j = 0; j < bufferLength; ++j)
						buffer[j] = reader.ReadByte();
				}

				data.Buffer = buffer;
				datas.Add(new Tuple<StoredQuestData, string>(data, name));
			}

			foreach (var item in datas) //Might be ordered properly already but I don't care
			{
				Quest q = QuestManager.Quests.FirstOrDefault(x => x.QuestName == item.Item2);

				if (q is null)
					ModContent.GetInstance<SpiritMod>().Logger.Debug($"No quest of name {item.Item2} exists on client.");
				else
				{
					StoredQuestData data = item.Item1;
					q.IsUnlocked = data.IsUnlocked;
					q.IsCompleted = data.IsCompleted;
					q.RewardsGiven = data.RewardsGiven;

					if (data.IsActive)
					{
						QuestManager.ActivateQuest(q);
						q.ReadFromDataBuffer(data.Buffer ?? Array.Empty<byte>()); //Extra security so I don't pass null
					}

					q.ActiveTime = data.TimeLeftActive;
					q.UnlockTime = data.TimeLeftUnlocked;
				}
			}
		}
	}
}
