using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Glyph
{
	public class DivineStrike : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Divine Strike");
			Description.SetDefault("Your next attack will deal increased damage");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}
	}
}