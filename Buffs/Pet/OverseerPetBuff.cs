using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class OverseerPetBuff : BasePetBuff<OverseerPet>
	{
		protected override (string, string) BuffInfo => ("Overseer", "'Looks like a final boss to me'");
	}
}