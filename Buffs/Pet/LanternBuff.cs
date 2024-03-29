using SpiritMod.Projectiles.Pet;

namespace SpiritMod.Buffs.Pet
{
	public class LanternBuff : BasePetBuff<LanternPet>
	{
		protected override (string, string) BuffInfo => ("Lantern Power Battery", "'It illuminates the way!'");
		protected override bool IsLightPet => true;
	}
}