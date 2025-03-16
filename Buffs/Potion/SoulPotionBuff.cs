using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Potion;

public class SoulPotionBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.pvpBuff[Type] = true;
		Main.buffNoTimeDisplay[Type] = false;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.endurance += 0.05f;
		player.GetSpiritPlayer().soulPotion = true;
	}
}
