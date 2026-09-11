using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritMod.Buffs
{
	class BeetleFortitude : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Beetle Fortitude");
			// Description.SetDefault("Each strike strenghtens you...");
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();
			player.endurance += modPlayer.beetleStacks * 0.01f;
		}

		public override bool ReApply(Player player, int time, int buffIndex)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();
			if (modPlayer.beetleStacks < 15) {
				modPlayer.beetleStacks++;
			}

			return false;
		}

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			MyPlayer modPlayer = Main.LocalPlayer.GetSpiritPlayer();
			tip = Language.GetTextValue("Mods.SpiritMod.Buffs.BeetleFortitude.Description", modPlayer.beetleStacks);
			rare = modPlayer.beetleStacks >> 1;
		}
	}
}
