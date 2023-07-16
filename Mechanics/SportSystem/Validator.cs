using Terraria.ModLoader;

namespace SpiritMod.Mechanics.SportSystem;

public abstract class Validator : ModType
{
	protected sealed override void Register() { }

	public abstract bool Validate(int x, int y, out int leftEdge, out int rightEdge, out int center, out int top, out int bottom);
}
