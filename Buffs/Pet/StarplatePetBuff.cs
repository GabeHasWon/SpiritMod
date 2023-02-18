using SpiritMod.Items.BossLoot.StarplateDrops.StarplatePet;

namespace SpiritMod.Buffs.Pet
{
	public class StarplatePetBuff : BasePetBuff<StarplatePetProjectile>
	{
		protected override (string, string) BuffInfo => ("Starplate Miniature", "Looking for something...");
	}
}