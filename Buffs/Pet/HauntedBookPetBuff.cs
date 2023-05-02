using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class HauntedBookPetBuff : BasePetBuff<HauntedBookPet>
	{
		protected override (string, string) BuffInfo => ("Haunted Tome", "'Haunted, yet comforting'");
	}
}