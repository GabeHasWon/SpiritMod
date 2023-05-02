using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class PhantomPetBuff : BasePetBuff<PhantomPet>
	{
		protected override (string, string) BuffInfo => ("Phantom", "'It blends into the night'");
	}
}