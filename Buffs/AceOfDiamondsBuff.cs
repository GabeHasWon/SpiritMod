using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Buffs
{
	public class AceOfDiamondsBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex) => player.GetDamage(DamageClass.Generic) += 0.20f;
	}
}
