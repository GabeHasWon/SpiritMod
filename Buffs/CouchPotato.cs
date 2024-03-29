using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Buffs
{
	public class CouchPotato : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Couch potato");
			// Description.SetDefault("'Stop being so lazy!'");
			Main.debuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Terraria.ID.BuffID.Sets.LongerExpertDebuff[Type] = true;
			Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += 6;
			player.moveSpeed *= 0.9f;
		}
	}
}
