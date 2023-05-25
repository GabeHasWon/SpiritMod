using SpiritMod.Projectiles.DonatorItems;

namespace SpiritMod.Buffs.Pet
{
	public class LoomingPresence : BasePetBuff<AbominationPet>
	{
		protected override (string, string) BuffInfo => ("Looming Presence", "It seems to attract a lot of attention.");
	}
}
