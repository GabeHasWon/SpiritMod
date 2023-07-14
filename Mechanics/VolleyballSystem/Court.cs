using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace SpiritMod.Mechanics.VolleyballSystem;

internal class Court
{
	public Rectangle bounds;
	public Point center;

	public Court(Rectangle bounds, Point center)
	{
		this.bounds = bounds;
		this.center = center;
	}

	internal void Save(TagCompound tag, int index)
	{
		string key = "court" + index;
		tag.Add(key + "Center", center.ToVector2());
		tag.Add(key + "Position", bounds.Location.ToVector2());
		tag.Add(key + "Size", bounds.Size());
	}

	internal static Court Load(TagCompound tag, int index)
	{
		string key = "court" + index;
		Point center = tag.Get<Vector2>(key + "Center").ToPoint();
		Point position = tag.Get<Vector2>(key + "Position").ToPoint();
		Point size = tag.Get<Vector2>(key + "Size").ToPoint();

		return new Court(new Rectangle(position.X, position.Y, size.X, size.Y), center);
	}
}
