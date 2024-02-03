using Terraria;
using Terraria.ModLoader;
using SpiritMod.GlobalClasses.Players;
using Terraria.Localization;

namespace SpiritMod.Items.Accessory.TalismanTree.SlagMedallion
{
	internal class SlagFury_buff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare) => tip = Language.GetText("Mods.SpiritMod.Buffs.SlagFury_buff.Description")
			.WithFormatArgs(Main.LocalPlayer.GetModPlayer<TalismanPlayer>().slagDamageMultiplier).Value;

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Generic) += player.GetModPlayer<TalismanPlayer>().slagDamageMultiplier;
		}
	}
}