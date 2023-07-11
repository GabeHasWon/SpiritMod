using Terraria;
using Terraria.ModLoader;
using SpiritMod.GlobalClasses.Players;

namespace SpiritMod.Items.Accessory.TalismanTree.GildedScarab
{
	internal class GildedScarab_buff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scarab Shield");
			Description.SetDefault("Defense increased by 0");
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		public override void ModifyBuffTip(ref string tip, ref int rare) => 
			tip = $"Defense increased by {Main.LocalPlayer.GetModPlayer<TalismanPlayer>().scarabDefense}";
		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += player.GetModPlayer<TalismanPlayer>().scarabDefense;
		}
	}
}
