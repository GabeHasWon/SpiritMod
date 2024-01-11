using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.QuestSystem.Quests;
using Terraria.Localization;

namespace SpiritMod.Items.Sets.MaterialsMisc.QuestItems
{
	[Sacrifice(1)]
	public class SeaMandrakeSac : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.Quest;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override bool OnPickup(Player player) => !player.HasItem(ModContent.ItemType<SeaMandrakeSac>());

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (!QuestManager.GetQuest<StylistQuestSeafoam>().IsCompleted)
			{
				TooltipLine line = new TooltipLine(Mod, "ItemName", Language.GetTextValue("Mods.SpiritMod.Items.DurasilkSheaf.CommonName")) {
					OverrideColor = new Color(100, 222, 122)
				};
				tooltips.Add(line);
			}
			TooltipLine line1 = new TooltipLine(Mod, "FavoriteDesc", Language.GetTextValue("Mods.SpiritMod.Items.SeaMandrakeSac.CustomTooltip")) {
				OverrideColor = new Color(255, 255, 255)
			};
			tooltips.Add(line1);
		}
	}
}
