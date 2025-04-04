using Terraria.ModLoader;

namespace SpiritMod.Tiles.Vanilla;

internal abstract class RandomUpdate : ILoadable
{
	public abstract void OnTick(int i, int j, int type);

	public virtual void Load(Mod mod) { }
	public virtual void Unload() { }
}
