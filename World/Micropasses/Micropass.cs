using System.Collections.Generic;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace SpiritMod.World.Micropasses;

public abstract class Micropass : ILoadable
{
	public abstract string WorldGenName { get; }

	public abstract void Run(GenerationProgress progress, GameConfiguration config);
	public abstract int GetWorldGenIndexInsert(List<GenPass> tasks, ref bool afterIndex);

	public virtual void Load(Mod mod) { }
	public virtual void Unload() { }
}
