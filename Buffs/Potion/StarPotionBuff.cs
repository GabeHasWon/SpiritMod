using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Potion
{
	public class StarPotionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star Burn");
			// Description.SetDefault("Critical hits may cause stars to fall from the sky\n4% increased ranged damage and critical strike chance when moving");
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.velocity != Vector2.Zero)
			{
				player.GetDamage(DamageClass.Ranged) += 0.04f;
				player.GetCritChance(DamageClass.Ranged) += 4;
			}
			player.GetSpiritPlayer().starBuff = true;
		}
	}
}
