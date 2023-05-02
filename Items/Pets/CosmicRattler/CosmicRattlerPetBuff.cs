using SpiritMod.Buffs.Pet;

namespace SpiritMod.Items.Pets.CosmicRattler
{
	public class CosmicRattlerPetBuff : BasePetBuff<CosmicRattlerPet>
	{
		protected override (string, string) BuffInfo => ("Starachnid", "'Inside it you can see the depths of space'");
	}
}