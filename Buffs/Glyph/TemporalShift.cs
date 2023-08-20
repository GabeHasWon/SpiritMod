using System;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Glyph
{
	public class TemporalShift : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Temporal Shift");
			// Description.SetDefault("Your movement speed is increased");
			Main.buffNoSave[Type] = true;
			Main.pvpBuff[Type] = true;
		}

		public override bool ReApply(Player player, int time, int buffIndex)
		{
			player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex] + time, 180);
			return false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.maxRunSpeed *= 1.5f;
			player.accRunSpeed *= 1.5f;
			player.runAcceleration *= 1.5f;
		}
	}
}
