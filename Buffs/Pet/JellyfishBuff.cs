using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class JellyfishBuff : BasePetBuff<JellyfishPet>
	{
		protected override (string, string) BuffInfo => ("Peaceful Jellyfish", "'The Jellyfish is helping you relax'");
	}
}