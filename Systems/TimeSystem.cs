using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Systems;

internal class TimeSystem : ILoadable
{
	/// <param name="day"> Whether it is day. use this instead of <see cref="Main.dayTime"/>. </param>
	public delegate void TimeDelegate(bool day);
	/// <summary> Invoked when the time changes from day to night or vise-versa, and before <see cref="MessageID.WorldData"/> is sent.<para/>
	/// Some mods that force time progression may prevent this from being invoked at all. </summary>
	public static event TimeDelegate TimeChanged;

	public void Load(Mod mod)
	{
		On_Main.UpdateTime_StartDay += StartDay;
		On_Main.UpdateTime_StartNight += StartNight;
	}

	private static void StartDay(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents)
	{
		TimeChanged?.Invoke(true);
		orig(ref stopEvents);
	}

	private static void StartNight(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
	{
		TimeChanged?.Invoke(false);
		orig(ref stopEvents);
	}

	public void Unload() => TimeChanged = null;
}
