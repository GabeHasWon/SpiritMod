using System;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.QuestSystem
{
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
		public Func<NPCSpawnInfo, bool> Conditions;

		/// <summary>
		/// Creates a new QuestPoolData struct.
		/// </summary>
		/// <param name="rate">New spawn weight for the given entity/entities. Set to null to use default rate.</param>
		/// <param name="exc">If true, increase spawns only if there is one existing NPC type alive</param>
		/// <param name="forced"> If true, the associated NPCs will spawn anywhere, anytime, always at the given rate.</param>
		/// <param name="cond">New spawn conditions for the NPC(s) to follow.</param>
		/// <exception cref="ArgumentException"/>
		public QuestPoolData(float? rate, bool exc = true, bool forced = false, Func<NPCSpawnInfo, bool> cond = null)
		{
			NewRate = rate;
			Exclusive = exc;
			Forced = forced;
			Conditions = cond;

			if (forced && !rate.HasValue)
				throw new ArgumentException("QuestPoolData created wherein forced is true but the rate is not set. Rate must be set in order for forced to work properly.");
		}
	}
}
