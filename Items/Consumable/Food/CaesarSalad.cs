using Humanizer;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Food
{
	public class CaesarSalad : FoodItem
	{
		internal override Point Size => new(30, 28);
		public override void StaticDefaults() => Tooltip.SetDefault("Minor improvements to all stats");

        public override bool CanUseItem(Player player)
        {
            player.AddBuff(BuffID.Regeneration, 3600);
            return true;
        }

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string flavourText = "'Maybe don't use a knife to eat this one'";

			if (Main.expertMode)
			{
				foreach (TooltipLine line in tooltips)
				{
					if (line.Mod == "Terraria" && line.Name == "WellFedExpert") //Replace the vanilla text with our own
						line.Text = $"{flavourText}\nGreatly increases life regeneration";
				}
			}
			else
			{
				tooltips.Insert(tooltips.Count - 1, new TooltipLine(Mod, string.Empty, $"{flavourText}\nIncreases life regeneration"));
			}
		}
	}
}
