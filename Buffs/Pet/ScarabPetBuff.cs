using SpiritMod.Items.BossLoot.ScarabeusDrops.ScarabPet;

namespace SpiritMod.Buffs.Pet
{
	public class ScarabPetBuff : BasePetBuff<ScarabPetProjectile>
	{
		protected override (string, string) BuffInfo => ("Tiny Scarab", "'It really loves to roll...'");
	}
}