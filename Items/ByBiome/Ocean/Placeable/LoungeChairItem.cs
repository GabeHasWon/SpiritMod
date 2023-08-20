using SpiritMod.Tiles.Furniture.Ocean;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.ByBiome.Ocean.Placeable;

[Sacrifice(1)]
public class LoungeChairItem : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<LoungeChair>());
		Item.value = Item.buyPrice(0, 0, 20, 0);
		Item.Size = new Microsoft.Xna.Framework.Vector2(26, 34);
	}
}