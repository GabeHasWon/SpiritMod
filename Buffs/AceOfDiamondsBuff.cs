using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs
{
	public class AceOfDiamondsBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ace of Diamonds");
			Description.SetDefault("Damage increased by 25%");
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Generic) += 0.25f;
		}
	}
}
