using System;
using Terraria.Localization;

namespace SpiritMod.Utilities
{
	public interface IBCRegistrable
	{
		void RegisterToChecklist(out BossChecklistDataHandler.EntryType entryType, out float progression,
			out string name, out Func<bool> downedCondition, ref BossChecklistDataHandler.BCIDData idData,
			ref LocalizedText spawnInfo, ref LocalizedText despawnMessage, ref string portrait, ref string headIcon,
			ref Func<bool> isAvailable);
	}
}