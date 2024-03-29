using SpiritMod.Tiles.Furniture.Paintings;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Paintings
{
	[Sacrifice(1)]
	public class TheGoodPaintingItem : ModItem
	{
		public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TheBadPaintingItem>();

		public override void SetDefaults()
		{
			Item.Size = new Microsoft.Xna.Framework.Vector2(48);
			Item.value = Item.buyPrice(0, 0, 40, 0);
			Item.rare = ItemRarityID.White;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<TheGoodPainting>();
		}
	}
}