using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class SaucerPetBuff : BasePetBuff<SaucerPet>
	{
		protected override (string, string) BuffInfo => ("Support Saucer", "'It seems to only provide moral support...'");
	}
}