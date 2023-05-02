using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class ShadowPetBuff : BasePetBuff<ShadowPet>
	{
		protected override (string, string) BuffInfo => ("Shadow Pup", "'Awww'");
	}
}