using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader.IO;

namespace SpiritMod.Mechanics.SportSystem;

internal class SportCourts : ModSystem
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	public List<Court> courts = new();

	public static bool TryAddCourt(Point pos, CourtGameTracker tracker)
	{
		if (!tracker.Validator.Validate(pos.X, pos.Y, out int left, out int right, out int netCenter, out int top, out int bottom))
			return false;

		RemoveCourt(new(pos.X, netCenter));
		ModContent.GetInstance<SportCourts>().courts.Add(new Court(new Rectangle(left, top, right - left, bottom - top), new Point(pos.X, netCenter), tracker));
		return true;
	}

	public static bool RemoveCourt(Point center)
	{
		var courts = ModContent.GetInstance<SportCourts>().courts;
		var court = courts.FirstOrDefault(x => x.bounds.Contains(center));

		if (court is not null)
		{
			courts.Remove(court);
			return true;
		}
		return false;
	}

	public static IEnumerable<Court> CourtsAt(Point center)
	{
		IEnumerable<Court> validCourts = ModContent.GetInstance<SportCourts>().courts.Where(x => x.bounds.Contains(center));
		return validCourts;
	}

	public static Court CourtAt<T>(Point center) where T : CourtGameTracker
	{
		var court = ModContent.GetInstance<SportCourts>().courts.FirstOrDefault(x => x.bounds.Contains(center) && x.tracker is T);
		return court;
	}

	public override void SaveWorldData(TagCompound tag)
	{
		tag.Add("volleyballCourtsCount", (short)courts.Count);

		for (int i = 0; i < courts.Count; ++i)
		{
			Court court = courts[i];
			court.Save(tag, i);
		}
	}

	public override void LoadWorldData(TagCompound tag)
	{
		if (tag.ContainsKey("volleyballCourtsCount"))
		{
			int count = tag.GetShort("volleyballCourtsCount");

			for (int i = 0; i < count; ++i)
				courts.Add(Court.Load(tag, i));
		}
	}

	public override void OnWorldUnload() => courts.Clear();

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Invasion Progress Bars"));

		layers.Insert(index, new LegacyGameInterfaceLayer(
			"SpiritMod: Volleyball Courts",
			delegate
			{
				foreach (var court in courts)
				{
					if (Vector2.DistanceSquared(court.center.ToWorldCoordinates(), Main.LocalPlayer.Center) < 3000 * 3000)
						court.Draw();
				}
				return true;
			},
			InterfaceScaleType.UI)
		);
	}
}
