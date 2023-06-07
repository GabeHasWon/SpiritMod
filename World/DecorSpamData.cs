namespace SpiritMod.World
{
	internal readonly struct DecorSpamData
	{
		public readonly int RealRepeats => (int)(BaseRepeats * GlobalExtensions.WorldSize);

		public readonly string Name;
		public readonly int[] Types;
		public readonly int[] ValidGround;
		public readonly int BaseRepeats;
		public readonly bool Forced;
		public readonly (int high, int low) RangeY;

		public DecorSpamData(string name, int[] types, int[] ground, int baseReps, (int high, int low) rangeY, bool forced = true)
		{
			Name = name;
			Types = types;
			ValidGround = ground;
			BaseRepeats = baseReps;
			RangeY = rangeY;
			Forced = forced;
		}
	}
}
