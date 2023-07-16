using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace SpiritMod.Mechanics.SportSystem;

public class Court
{
	/// <summary>
	/// Shorthand for tracker.Active.
	/// </summary>
	public bool Active => tracker.Active;

	public Rectangle bounds;
	public Point center;
	public CourtGameTracker tracker;

	public Court(Rectangle bounds, Point center, CourtGameTracker tracker)
	{
		this.bounds = bounds;
		this.center = center;
		this.tracker = tracker;
	}

	public void Draw() => tracker.Draw(this);

	internal void Save(TagCompound tag, int index)
	{
		TagCompound court = new TagCompound();

		court.Add("center", center.ToVector2());
		court.Add("position", bounds.Location.ToVector2());
		court.Add("size", bounds.Size());
		court.Add("trackerType", tracker.GetType().FullName);
		tracker.SaveData(court);

		tag.Add("court" + index, court);
	}

	internal static Court Load(TagCompound tag, int index)
	{
		TagCompound court = tag.GetCompound("court" + index);
		Point center = court.Get<Vector2>("center").ToPoint();
		Point position = court.Get<Vector2>("position").ToPoint();
		Point size = court.Get<Vector2>("size").ToPoint();
		string trackerType = court.GetString("trackerType");

		if (Activator.CreateInstance(Type.GetType(trackerType)) is not CourtGameTracker tracker)
			throw new NullReferenceException("CourtGameTracker invalid load! Provided tracker name: " + trackerType);

		tracker.LoadData(court);
		return new Court(new Rectangle(position.X, position.Y, size.X, size.Y), center, tracker);
	}
}
