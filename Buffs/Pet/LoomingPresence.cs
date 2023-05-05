using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.DonatorItems;
using SpiritMod.Projectiles.Pet;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Pet
{
	public class LoomingPresence : BasePetBuff<DemonicBlob>
	{
		protected override (string, string) BuffInfo => ("Looming Presence", "It seems to attract a lot of attention.");
	}
}
