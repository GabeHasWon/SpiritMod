using SpiritMod.Items.BossLoot.VinewrathDrops.VinewrathPet;

namespace SpiritMod.Buffs.Pet
{
	public class VinewrathPetBuff : BasePetBuff<VinewrathPetProjectile>
	{
		protected override (string, string) BuffInfo => ("Wrathful Seedling", "");
	}
}