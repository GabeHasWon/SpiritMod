using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class CaltfistPetBuff : BasePetBuff<Caltfist>
	{
		protected override (string, string) BuffInfo => ("Cultfish", "This little bugger lights the way!");
		protected override bool IsLightPet => true;
	}
}