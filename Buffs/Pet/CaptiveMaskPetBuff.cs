using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class CaptiveMaskPetBuff : BasePetBuff<CaptiveMaskPet>
	{
		protected override (string, string) BuffInfo => ("Unbound Mask", "'Once more unto the breach!'");
	}
}