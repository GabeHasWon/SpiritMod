using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class SwordPetBuff : BasePetBuff<SwordPet>
	{
		protected override (string, string) BuffInfo => ("Possessed Blade", "'Is this a dagger I see in front of me?'");
	}
}