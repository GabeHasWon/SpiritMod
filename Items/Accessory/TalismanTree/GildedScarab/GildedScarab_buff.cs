using Terraria;
using Terraria.ModLoader;
using SpiritMod.GlobalClasses.Players;
using Terraria.Localization;

namespace SpiritMod.Items.Accessory.TalismanTree.GildedScarab;

internal class GildedScarab_buff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.pvpBuff[Type] = true;
		Main.buffNoTimeDisplay[Type] = false;
	}

	public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare) 
		=> tip = Language.GetText("Mods.SpiritMod.Buffs.GildedScarab_buff.Description").WithFormatArgs(Main.LocalPlayer.GetModPlayer<TalismanPlayer>().scarabDefense).Value;
	
	public override void Update(Player player, ref int buffIndex) => player.statDefense += player.GetModPlayer<TalismanPlayer>().scarabDefense;
}