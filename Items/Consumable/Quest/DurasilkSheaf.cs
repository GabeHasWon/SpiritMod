using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.QuestSystem.Quests;
using Terraria.Localization;

namespace SpiritMod.Items.Consumable.Quest
{
	[Sacrifice(1)]
	public class DurasilkSheaf : ModItem
	{
		public override void SetDefaults()
		{
			Item.autoReuse = false;
			Item.useTurn = true;
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.buyPrice(0, 0, 3, 0);
			Item.createTile = ModContent.TileType<Tiles.Furniture.DurasilkSheafTile>();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (!QuestManager.GetQuest<FirstAdventure>().IsCompleted)
			{
				TooltipLine line = new TooltipLine(Mod, "ItemName", Language.GetTextValue("Mods.SpiritMod.Items.DurasilkSheaf.CommonName")) {
					OverrideColor = new Color(100, 222, 122)
				};
				tooltips.Add(line);
			}
			TooltipLine line1 = new TooltipLine(Mod, "FavoriteDesc", Language.GetTextValue("Mods.SpiritMod.Items.DurasilkSheaf.CustomTooltip")) {
				OverrideColor = new Color(255, 255, 255)
			};
			tooltips.Add(line1);
		}
	}
}
