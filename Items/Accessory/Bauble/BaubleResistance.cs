using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Bauble
{
	public class BaubleResistance : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bauble Resistance");
			Description.SetDefault("You feel protected");
			Main.buffNoSave[Type] = true;
		}
	}
}
