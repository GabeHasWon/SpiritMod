using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritMod.Buffs.Armor
{
	class ExplorerFight : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Strike Strength");
			// Description.SetDefault("You're getting the hang of it!");
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();
			player.GetDamage(DamageClass.Generic) += modPlayer.damageStacks * 0.03f;
		}

		public override bool ReApply(Player player, int time, int buffIndex)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();
			if (modPlayer.damageStacks < 4) {
				modPlayer.damageStacks++;
			}

			return false;
		}

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			MyPlayer modPlayer = Main.LocalPlayer.GetSpiritPlayer();
			tip = Language.GetTextValue("Mods.SpiritMod.Buffs.ExplorerFight.Description", modPlayer.damageStacks * 3);
			rare = modPlayer.damageStacks >> 1;
		}
	}
}
