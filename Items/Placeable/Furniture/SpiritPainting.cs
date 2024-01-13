using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritPaintingTile = SpiritMod.Tiles.Furniture.SpiritPainting;
using Terraria.Localization;

namespace SpiritMod.Items.Placeable.Furniture
{
	[Sacrifice(1)]
	public class SpiritPainting : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 94;
			Item.height = 62;
			Item.value = Item.value = Item.buyPrice(0, 10, 1000, 10);
			Item.rare = ItemRarityID.LightPurple;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<SpiritPaintingTile>();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(Mod, "ItemName", Language.GetTextValue("Mods.SpiritMod.Items.SpiritPainting.CustomTooltip"));
			line.OverrideColor = new Color(50, 80, 200);
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips) {
				if (line2.Mod == "Terraria" && line2.Name == "ItemName") {
					line2.OverrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
				}
			}
		}
	}
}