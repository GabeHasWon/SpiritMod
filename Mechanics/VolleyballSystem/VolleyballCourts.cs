using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader.IO;

namespace SpiritMod.Mechanics.VolleyballSystem;

internal class VolleyballCourts : ModSystem
{
	public List<Court> courts = new();

	public static bool TryAddCourt(Point pos)
	{
		if (!Validator.Validate(pos.X, pos.Y, out int left, out int right, out int netCenter, out int top, out int bottom))
			return false;

		RemoveCourt(new(pos.X, netCenter));
		ModContent.GetInstance<VolleyballCourts>().courts.Add(new Court(new Rectangle(left, top, right - left, bottom - top), new Point(pos.X, netCenter)));
		return true;
	}

	public static bool RemoveCourt(Point center)
	{
		var courts = ModContent.GetInstance<VolleyballCourts>().courts;
		var court = courts.FirstOrDefault(x => x.bounds.Contains(center));

		if (court is not null)
		{
			courts.Remove(court);
			return true;
		}
		return false;
	}

	public override void SaveWorldData(TagCompound tag)
	{
		tag.Add("volleyballCourtsCount", (short)courts.Count);

		for (int i = 0; i < courts.Count; ++i)
		{
			Court court = courts[i];
			court.Save(tag, i);
		}

		courts.Clear();
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
						CourtDrawer.Draw(court);
				}
				return true;
			},
			InterfaceScaleType.UI)
		);
	}
}
