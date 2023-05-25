using SpiritMod.Projectiles.DonatorItems;

namespace SpiritMod.Buffs.Pet
{
	public class FaeriePetBuff : BasePetBuff<FaeriePet>
	{
		protected override (string, string) BuffInfo => ("Waning Gibbous", "The Moonlit Faerie will protect you");
	}
}