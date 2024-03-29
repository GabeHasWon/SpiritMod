using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritMod.Items.Consumable.Quest
{
	[Sacrifice(1)]
	public class ExplorerScrollHiveFull : ModItem
	{
		public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 30;
			Item.height = 20;
            Item.createTile = ModContent.TileType<Tiles.Furniture.Paintings.HiveMap>();
			Item.value = Item.buyPrice(0, 0, 50, 0);
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line1 = new TooltipLine(Mod, "FavoriteDesc", Language.GetTextValue("Mods.SpiritMod.Items.ExplorerScrollHiveFull.CustomTooltip"));
			line1.OverrideColor = new Color(255, 255, 255);
			tooltips.Add(line1);
		}
	}
}
