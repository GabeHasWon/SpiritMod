using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Tiles
{
	public class PagodaCurse : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Curse of the Pagoda");
			Description.SetDefault("You feel a ghostly presence nearby");
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}
	}
}
