using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Systems;

public sealed class TimeSystem : ModSystem
{
	/// <param name="day"> Whether it is day. use this instead of <see cref="Main.dayTime"/>. </param>
	public delegate void TimeDelegate(bool day);
	/// <summary> Invoked when the time changes from day to night or vise-versa, and before <see cref="MessageID.WorldData"/> is sent.<para/>
	/// Some mods that force time progression may prevent this from being invoked at all. </summary>
	public static event TimeDelegate TimeChanged;

	private bool _wasDayTime;

	public override void OnWorldLoad() => _wasDayTime = Main.dayTime;

	public override void PostUpdateEverything()
	{
		if (Main.dayTime != _wasDayTime)
			TimeChanged?.Invoke(Main.dayTime);

		_wasDayTime = Main.dayTime;
	}

	public override void Unload() => TimeChanged = null;
}
