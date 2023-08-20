using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs
{
	public class FateBlessing : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Fate's Blessing");
			// Description.SetDefault("You are protected by the fates");
			Main.buffNoSave[Type] = true;
			Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
		}
	}
}
