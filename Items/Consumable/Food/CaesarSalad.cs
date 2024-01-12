using Humanizer;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class CaesarSalad : FoodItem
	{
		internal override Point Size => new(30, 28);

        public override bool CanUseItem(Player player)
        {
            player.AddBuff(BuffID.Regeneration, 3600);
            return true;
        }

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{

			if (Main.expertMode)
			{
				foreach (TooltipLine line in tooltips)
				{
					if (line.Mod == "Terraria" && line.Name == "WellFedExpert") //Replace the vanilla text with our own
						line.Text = Language.GetTextValue("Mods.SpiritMod.Items.CaesarSalad.CustomTooltip1");
				}
			}
			else
			{
				tooltips.Insert(tooltips.Count - 1, new TooltipLine(Mod, string.Empty, Language.GetTextValue("Mods.SpiritMod.Items.CaesarSalad.CustomTooltip2")));
			}
		}
	}
}
