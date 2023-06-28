using System;
using System.IO;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem
{
	[Serializable]
	public struct QuestPoolData
	{
		/// <summary>
		/// New spawn weight for the given entity/entities. Set to null to use default rate.
		/// </summary>
		public float? NewRate;

		/// <summary>
		/// If true, increase spawns only if there is one existing NPC type alive
		/// </summary>
		public bool Exclusive;

		/// <summary>
		/// If true, the associated NPCs will spawn anywhere, anytime, always at the given rate.
		/// </summary>
		public bool Forced;

		/// <summary>
		/// New spawn conditions for the NPC(s) to follow.
		/// </summary>
		public string ConditionKey;

		/// <summary>
		/// Identifier for this given PoolData. Used for syncing and deletion.
		/// </summary>
		public long ID;

		/// <summary>
		/// Creates a new QuestPoolData struct.<br/>
		/// If you use a <paramref name="condition"/>, provide a key to a method that takes <see cref="NPCSpawnInfo"/> arg and returns <see cref="bool"/>.
		/// </summary>
		/// <param name="rate">New spawn weight for the given entity/entities. Set to null to use default rate.</param>
		/// <param name="exc">If true, increase spawns only if there is one existing NPC type alive</param>
		/// <param name="forced"> If true, the associated NPCs will spawn anywhere, anytime, always at the given rate.</param>
		/// <param name="condition">New spawn conditions for the NPC(s) to follow.</param>
		/// <exception cref="ArgumentException"/>
		public QuestPoolData(float? rate, bool exc = true, bool forced = false, string condition = null)
		{
			NewRate = rate;
			ID = DateTime.Now.ToBinary();
			Exclusive = exc;
			Forced = forced;
			ConditionKey = condition;

			if (forced && !rate.HasValue)
				throw new ArgumentException("QuestPoolData created wherein forced is true but the rate is not set. Rate must be set in order for forced to work properly.");
		}

		internal void Serialize(ModPacket packet)
		{
			packet.Write(NewRate is null ? (Half)(-1) : (Half)NewRate.Value);
			packet.Write(Exclusive);
			packet.Write(Forced);
			packet.Write(ConditionKey ?? "");
			packet.Write(ID);
		}

		internal static QuestPoolData Deserialize(BinaryReader reader)
		{
			Half half = reader.ReadHalf();
			float? rate = half == (Half)(-1) ? null : (float)half;
			bool excl = reader.ReadBoolean();
			bool forced = reader.ReadBoolean();
			string conditionKey = reader.ReadString();
			long id = reader.ReadInt64();
			var data = new QuestPoolData(rate, excl, forced, conditionKey)
			{
				ID = id
			};
			return data;
		}
	}
}
