﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.UI.Elements;
using Terraria.Localization;
using SpiritMod.NPCs.Town;
using System.Linq;
using Terraria.Chat;

namespace SpiritMod.Mechanics.QuestSystem
{
	public abstract class Quest
	{
		protected TaskBuilder _tasks;
		protected QuestTask _currentTask;
		private bool _questActive;
		private bool _questUnlocked;
		private bool _questCompleted;
		private bool _rewardsGiven;
		private int _completedCounter = 0;
		private bool _previouslyUnavailable = false;

		internal List<string> _altNames;

		public QuestTask CurrentTask => _currentTask;

		/// <summary>
		/// Used for localization (i.e. <see cref="FullLangKey"/>) and saving/loading, as <see cref="QuestName"/> is a localized value.
		/// </summary>
		public virtual string QuestKey => GetType().Name;

		/// <summary>
		/// Full path: <code>"Mods.SpiritMod.Quests.QuestInfo." + <see cref="QuestKey"/>;</code>
		/// </summary>
		public string FullLangKey => "Mods.SpiritMod.Quests.QuestInfo." + QuestKey;

		/// <summary>
		/// Quest-only path, mainly for use in <see cref="QuestManager.Localization(string)"/> or <see cref="QuestManager.LocalizationValue(string)"/>.<br/>
		/// Path: <code>"QuestInfo." + <see cref="QuestKey"/>;</code>
		/// </summary>
		public string ShortLangKey => "QuestInfo." + QuestKey;

		public string QuestName => Language.GetTextValue(FullLangKey + ".Name");
		public string QuestDescription => Language.GetTextValue(FullLangKey + ".Description");

		public virtual int QuestClientID => ModContent.NPCType<Adventurer>();
		public virtual string QuestClientOverride => null;
		public string QuestClient => QuestClientOverride ?? QuestManager.LocalizationValue("The") + Lang.GetNPCNameValue(QuestClientID);

		public virtual string QuestCategory => "";

		public virtual int Difficulty => 1;
		public virtual (int, int)[] QuestRewards => null;
		public virtual bool TutorialActivateButton => false;
		public virtual Texture2D QuestImage { get; set; }

		public virtual bool AppearsWhenUnlocked => true;
		public virtual bool CountsTowardsQuestTotal => true;
		public virtual bool AnnounceRelocking => false;
		public virtual bool LimitedUnlock => false;
		public virtual bool AnnounceDeactivation => false;
		public virtual bool LimitedActive => false;
		public int UnlockTime { get; set; }
		public int ActiveTime { get; set; }

		public int QuestCategoryIndex => QuestManager.GetCategoryInfo(QuestCategory).Index;

		public bool IsActive
		{
			get => _questActive; set
			{
				_questActive = value;
				OnQuestStateChanged?.Invoke(this);

				if (value)
					OnActivate();
				else
					OnDeactivate();
			}
		}

		public virtual bool IsUnlocked { get => _questUnlocked; set { _questUnlocked = value; OnQuestStateChanged?.Invoke(this); } }
		public bool IsCompleted { get => _questCompleted; set { _questCompleted = value; OnQuestStateChanged?.Invoke(this); } }
		public bool RewardsGiven { get => _rewardsGiven; set { _rewardsGiven = value; OnQuestStateChanged?.Invoke(this); } }
		public int WhoAmI { get; set; }

		public event Action<Quest> OnQuestStateChanged;

		public Quest()
		{
			_altNames = new List<string>();
			_tasks = new TaskBuilder();
		}

		public LocalizedText GetText(string key) => QuestManager.Localization(ShortLangKey + "." + key);

		public virtual string GetObjectivesBook()
		{
			var lines = new List<string>();
			var final = new List<(string, bool)>();

			for (QuestTask task = _tasks.Start; task != null; task = task.NextTask)
			{
				task.GetBookText(lines);
				foreach (string s in lines)
				{
					final.Add((s, task.Completed));
				}
				lines.Clear();
			}

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < final.Count; i++)
			{
				if (!final[i].Item2) 
					builder.Append("[c/2B1C11:");
				else 
					builder.Append("[c/928269:");

				builder.Append("- ").Append(final[i].Item1).Append(']');

				if (i < final.Count - 1) 
					builder.Append('\n');
			}

			return builder.ToString();
		}

		public virtual string GetObjectivesHUD()
		{
			var lines = new List<string>();
			_currentTask.GetHUDText(lines);

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < lines.Count; i++)
			{
				builder.Append("- ").Append(lines[i]);

				if (i < lines.Count - 1) builder.Append('\n');
			}

			return builder.ToString();
		}

		public virtual void OnQuestComplete()
		{
			IsCompleted = true;
			QuestManager.SayInChat(QuestManager.Localization("QuestCompleteChat").WithFormatArgs(WhoAmI, QuestName), Color.White, true);
		}

		public virtual void OnUnlock() { }

		public virtual void OnActivate()
		{
			_currentTask = _tasks.Start;

			_currentTask.Activate(this);
		}

		public virtual void OnDeactivate()
		{
			if (_currentTask != null)
			{
				_currentTask.Deactivate();
			}
		}

		public virtual void ResetEverything()
		{
			ResetAllProgress();
			IsActive = false;
			IsCompleted = false;
			IsUnlocked = false;
			RewardsGiven = false;
		}

		public virtual void ResetAllProgress()
		{
			for (QuestTask task = _tasks.Start; task != null; task = task.NextTask)
			{
				task.ResetProgress();
			}
		}

		public virtual void UpdateBookOverlay(UIShaderImage image) => image.Texture = null;

		public virtual void Update()
		{
			if (LimitedActive)
			{
				ActiveTime--;
				if (ActiveTime <= 0)
				{
					if (AnnounceDeactivation)
						QuestManager.SayInChat(QuestManager.Localization("QuestOutOfTimeChat").WithFormatArgs(WhoAmI, QuestName), Color.White);

					QuestManager.DeactivateQuest(this);
				}
			}

			if (_currentTask.CheckCompletion())
			{
				_completedCounter++; //This counter is here so the HUD works. that's all.

				if (_completedCounter >= 3)
					RunCompletion();
			}

			if (IsQuestPossible() && _previouslyUnavailable)
			{
				_previouslyUnavailable = false;
				QuestManager.SayInChat(QuestManager.Localization("NewQuestChat").WithFormatArgs(WhoAmI, QuestName), Color.White);
			}
		}

		internal void RunCompletion(int taskArgument = byte.MaxValue)
		{
			if (IsCompleted)
				return;

			// Set branched tasks's next task to complete properly
			// This is hacky and I don't like it but it should work
			if (QuestManager.Quiet && _currentTask is BranchingTask branchTask && taskArgument != byte.MaxValue)
				_currentTask = branchTask.Tasks.ElementAt(taskArgument);

			if (_currentTask is not null) //This should only be true when the quest is force-completed
			{
				if (!QuestManager.Quiet && Main.netMode == NetmodeID.MultiplayerClient) //Tell server to progress the quest.
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Quest, 5);
					packet.Write((byte)QuestMessageType.ProgressOrComplete);
					packet.Write(true);
					packet.Write(QuestName);

					if (_currentTask is not BranchingTask branch)
						packet.Write(byte.MaxValue);
					else
						packet.Write(branch.taskSlotForMP);

					packet.Write((byte)Main.myPlayer);
					packet.Send();
				}

				_currentTask.Completed = true;
				_currentTask.Deactivate();
				_currentTask = _currentTask.NextTask;
			}

			if (_currentTask is null) //Quest completed
			{
				OnQuestComplete();
				QuestManager.DeactivateQuest(this);
			}
			else //Quest continues
				_currentTask.Activate(this);
		}

		public virtual bool IsQuestPossible() => true;

		public virtual void OnMPSync()
		{
			for (QuestTask task = _tasks.Start; task != null; task = task.NextTask)
			{
				task.OnMPSyncTick();
			}
		}

		public virtual byte[] GetTaskDataBuffer()
		{
			byte[] buffer = new byte[16];
			using (var stream = new MemoryStream(buffer))
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(_currentTask.TaskID);
					_currentTask.WriteData(writer);
				}
			}
			return buffer;
		}

		public virtual void ReadFromDataBuffer(byte[] buffer)
		{
			using (var stream = new MemoryStream(buffer))
			{
				using (var reader = new BinaryReader(stream))
				{
					int taskId = reader.ReadInt32();

					_currentTask = _tasks[taskId];
					_currentTask.ReadData(reader);
					_currentTask.Activate(this); //Fixes branch quests being poorly reloaded
				}
			}
		}

		public void GiveRewards()
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			foreach (var itemPair in QuestRewards)
				ItemUtils.NewItemWithSync(Main.LocalPlayer.GetSource_GiftOrReward(), Main.myPlayer, Main.LocalPlayer.getRect(), itemPair.Item1, itemPair.Item2);
		}

		public void ModifySpawnRateUnique(IDictionary<int, float> pool, int id, float rate)
		{
			if (pool.ContainsKey(id) && pool[id] > 0f && !NPC.AnyNPCs(id))
				pool[id] = rate;
		}

		// TODO: Write Packet and ReadPacket
		// TODO: Have each quest section have it's own WritePacket and ReadPacket
	}
}
