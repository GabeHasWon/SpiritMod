using SpiritMod.Buffs.Pet;
using Terraria.ModLoader;

namespace SpiritMod.Items.Pets.CosmicRattler
{
	public class CosmicRattlerPetBuff : BasePetBuff<CosmicRattlerPet>
	{
		public override bool IsLoadingEnabled(Mod mod) => false;
		protected override (string, string) BuffInfo => ("Starachnid", "'Inside it you can see the depths of space'");
	}
}