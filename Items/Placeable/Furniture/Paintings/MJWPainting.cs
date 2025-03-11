using SpiritMod.Tiles.Furniture.Paintings;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Paintings
{
    [Sacrifice(1)]
    public class MJWPainting : ModItem
    {

        public override void SetDefaults()
        {
            Item.height = 34;
            Item.width = 60;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.White;
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<MJWPainting_Tile>();
        }
    }
}