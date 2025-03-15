using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Armor;

public class FrigidCooldown : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}
}