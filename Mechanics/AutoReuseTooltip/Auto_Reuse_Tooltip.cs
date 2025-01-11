using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using SpiritMod.Utilities;
using Terraria.Localization;

namespace SpiritMod.Mechanics.AutoReuseTooltip;

public class Auto_Reuse_Tooltip : GlobalItem
{
	public override bool InstancePerEntity => true;

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		var config = ModContent.GetInstance<SpiritClientConfig>();

		if (config.AutoReuse && item.ammo == 0 && !item.accessory)
		{
			bool? hookCanAutoreuse = ItemLoader.CanAutoReuseItem(item, Main.LocalPlayer);
			bool autoReuse = hookCanAutoreuse is null ? item.autoReuse || item.channel : hookCanAutoreuse.Value;

			if (autoReuse && item.IsRanged() && !Main.SettingsEnabled_AutoReuseAllItems)
			{
				TooltipLine line = new TooltipLine(Mod, "isAutoreused", Language.GetTextValue("Mods.SpiritMod.MiscUI.Autofire") + " [c/64FF64:✔]");
				tooltips.Add(line);
			}
			else if (!autoReuse && item.IsRanged())
			{
				TooltipLine line2 = new TooltipLine(Mod, "isntAutoreused", Language.GetTextValue("Mods.SpiritMod.MiscUI.Autofire") + " [c/FF505A:✘]");
				tooltips.Add(line2);
			}	

			if (autoReuse && item.IsMelee() && !Main.SettingsEnabled_AutoReuseAllItems)
			{
				TooltipLine line = new TooltipLine(Mod, "isAutoreused", Language.GetTextValue("Mods.SpiritMod.MiscUI.Autoswing") + " [c/64FF64:✔]");
				tooltips.Add(line);
			}
			else if (!autoReuse && item.IsMelee())
			{
				TooltipLine line2 = new TooltipLine(Mod, "isntAutoreused", Language.GetTextValue("Mods.SpiritMod.MiscUI.Autoswing") + " [c/FF505A:✘]");
				tooltips.Add(line2);
			}	

			if (autoReuse && item.IsMagic() && !Main.SettingsEnabled_AutoReuseAllItems)
			{
				TooltipLine line = new TooltipLine(Mod, "isAutoreused", Language.GetTextValue("Mods.SpiritMod.MiscUI.Autofire") + " [c/64FF64:✔]");
				tooltips.Add(line);
			}
			else if (!autoReuse && item.IsMagic())
			{
				TooltipLine line2 = new TooltipLine(Mod, "isntAutoreused", Language.GetTextValue("Mods.SpiritMod.MiscUI.Autofire") + " [c/FF505A:✘]");
				tooltips.Add(line2);
			}	

			if (autoReuse && item.IsSummon() && !Main.SettingsEnabled_AutoReuseAllItems)
			{
				TooltipLine line = new TooltipLine(Mod, "isAutoreused", Language.GetTextValue("Mods.SpiritMod.MiscUI.Autofire") + " [c/64FF64:✔]");
				tooltips.Add(line);
			}
			else if (!autoReuse && item.IsSummon())
			{
				TooltipLine line2 = new TooltipLine(Mod, "isntAutoreused", Language.GetTextValue("Mods.SpiritMod.MiscUI.Autofire") + " [c/FF505A:✘]");
				tooltips.Add(line2);
			}
		}
	}
}
