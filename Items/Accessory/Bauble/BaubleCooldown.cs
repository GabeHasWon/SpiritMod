using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Bauble
{
	public class BaubleCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bauble Cooldown");
			Description.SetDefault("Bauble's shield is on cooldown");
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.persistentBuff[Type] = true;
			Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
	}
}
