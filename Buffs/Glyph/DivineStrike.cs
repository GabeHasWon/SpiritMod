using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Glyph
{
	public class DivineStrike : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}
	}
}