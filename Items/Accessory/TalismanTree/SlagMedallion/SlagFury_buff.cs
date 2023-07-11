using Terraria;
using Terraria.ModLoader;
using SpiritMod.GlobalClasses.Players;


namespace SpiritMod.Items.Accessory.TalismanTree.SlagMedallion
{
	internal class SlagFury_buff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slag Fury");
			Description.SetDefault("Damage increased by 0%");
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		public override void ModifyBuffTip(ref string tip, ref int rare) =>
			tip = $"Damage increased by {System.Math.Truncate(Main.LocalPlayer.GetModPlayer<TalismanPlayer>().slagDamageMultiplier * 100)}%";
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Generic) += player.GetModPlayer<TalismanPlayer>().slagDamageMultiplier;
		}
	}
}
