using SpiritMod.Projectiles.DonatorItems;

namespace SpiritMod.Buffs.Pet
{
	public class HarpyPetBuff : BasePetBuff<HarpyPet>
	{
		protected override (string, string) BuffInfo => ("Waning Gibbous", "The Moonlit Faerie will protect you");
	}
}