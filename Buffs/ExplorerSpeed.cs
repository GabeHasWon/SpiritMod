using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs
{
	public class ExplorerSpeed : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Explorer's Speed");
			// Description.SetDefault("Your mobility is greatly improved");
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.maxRunSpeed *= 1.75f;
			player.accRunSpeed *= 1.75f;
			player.runAcceleration *= 1.75f;
		}
	}
}
