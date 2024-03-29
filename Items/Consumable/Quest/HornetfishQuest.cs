using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem.Quests;
using SpiritMod.Mechanics.QuestSystem;
using Terraria.Localization;

namespace SpiritMod.Items.Consumable.Quest
{
	[Sacrifice(1)]
	public class HornetfishQuest : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = -11;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (!QuestManager.GetQuest<ItsNoSalmon>().IsCompleted)
			{
				TooltipLine line = new TooltipLine(Mod, "ItemName", Language.GetTextValue("Mods.SpiritMod.Items.DurasilkSheaf.CommonName"));
				line.OverrideColor = new Color(100, 222, 122);
				tooltips.Add(line);
			}
			TooltipLine line1 = new TooltipLine(Mod, "FavoriteDesc", Language.GetTextValue("Mods.SpiritMod.Items.HornetfishQuest.CustomTooltip"));
			line1.OverrideColor = new Color(255, 255, 255);
			tooltips.Add(line1);
		}
	}
}
