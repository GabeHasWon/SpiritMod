using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class BladePetBuff : BasePetBuff<BladePet>
	{
		protected override (string, string) BuffInfo => ("Possessed Blade", "'Is this a dagger I see in front of me?'");
	}
}