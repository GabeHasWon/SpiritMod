using SpiritMod.Items.BossLoot.InfernonDrops.InfernonPet;

namespace SpiritMod.Buffs.Pet
{
	public class InfernonPetBuff : BasePetBuff<InfernonPetProjectile>
	{
		protected override (string, string) BuffInfo => ("Inferno", "'You'd better stay out of his way'");
	}
}