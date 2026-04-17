using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs;

public class SpiritBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.pvpBuff[Type] = true;
		Main.buffNoTimeDisplay[Type] = false;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		MyPlayer modPlayer = player.GetSpiritPlayer();
		player.GetDamage(DamageClass.Generic) += 0.05f;
		player.GetCritChance(DamageClass.Generic) += 5;
		modPlayer.spiritBuff = true;
	}
}
