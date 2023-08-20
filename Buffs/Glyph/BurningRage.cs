using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Glyph
{
	public class BurningRage : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Burning Rage");
			// Description.SetDefault("You're taking damage");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.lifeRegen -= 15;

			if (Main.rand.NextBool())
			{
				int dust = Dust.NewDust(player.position, player.width, player.height, DustID.Torch);
				Main.dust[dust].scale = Main.rand.NextFloat(1.4f, 2.4f);
				Main.dust[dust].velocity.Y += Main.rand.NextFloat(0, -2f);
				Main.dust[dust].noGravity = true;
			}
		}
	}
}
