using System;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.Mechanics.SportSystem;

public abstract class CourtGameTracker : ModType
{
	public struct PerSide<T>
	{
		public T left;
		public T right;

		public PerSide()
		{
			left = default;
			right = default;
		}

		public PerSide(T left, T right)
		{
			this.left = left;
			this.right = right;
		}
	}

	public abstract Validator Validator { get; }

	public bool Active { get; private set; }

	public PerSide<short> points;
	public PerSide<short> wins;

	protected sealed override void Register() { }

	public void Start()
	{
		Active = true;
		points = new();

		InternalStart();
	}

	public void End()
	{
		Active = false;

		InternalEnd();
	}

	public abstract void Draw(Court court);

	protected abstract void InternalStart();
	protected abstract void InternalEnd();

	public virtual void OnScorePoint(bool left)
	{
		if (left)
			points.left++;
		else
			points.right++;

		CheckWin();
	}

	public virtual void CheckWin()
	{
		if (points.left > 3)
		{
			points.left = 0;
			wins.left++;
		}

		if (points.right > 3)
		{
			points.right = 0;
			wins.right++;
		}
	}

	public virtual void SaveData(TagCompound tag)
	{
		tag.Add("leftWins", wins.left);
		tag.Add("rightWins", wins.right);
	}

	public virtual void LoadData(TagCompound tag) => wins = new PerSide<short>(tag.GetShort("leftWins"), tag.GetShort("rightWins"));
}