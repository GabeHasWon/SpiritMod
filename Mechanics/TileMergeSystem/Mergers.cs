using System.Collections.Generic;
using System.Linq;
using Terraria.ID;

namespace SpiritMod.Mechanics.TileMergeSystem
{
	internal class Mergers
	{
		private static Dictionary<int, List<int>> mergers = new();
		private static bool[] universalMergers = TileID.Sets.Factory.CreateBoolSet(false, TileID.Dirt);

		public static void AddMergers(int baseType, params int[] types)
		{
			if (mergers.ContainsKey(baseType))
				mergers.Add(baseType, types.ToList());
			else
				mergers[baseType].AddRange(types.ToList());
		}

		public static void AddUniversalMerger(int type) => mergers[type].Add(type);

		public static bool Has(int type) => mergers.ContainsKey(type);
		public static bool TypeHas(int type, int value) => mergers.ContainsKey(type) && mergers[type].Contains(value);

		public static bool HasUniversal(int type) => universalMergers[type];
	}
}
