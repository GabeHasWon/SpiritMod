using SpiritMod.Items.BossLoot.MoonWizardDrops.MJWPet;

namespace SpiritMod.Buffs.Pet
{
	public class MJWPetBuff : BasePetBuff<MJWPetProjectile>
	{
		protected override (string, string) BuffInfo => ("Moon Jelly Lightbulb", "'No installation necessary!'");
		protected override bool IsLightPet => true;
	}
}