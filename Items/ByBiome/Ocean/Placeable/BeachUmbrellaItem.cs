using SpiritMod.Tiles.Furniture.Ocean;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.ByBiome.Ocean.Placeable;

[Sacrifice(1)]
public class BeachUmbrellaItem : ModItem
{
	// public override void SetStaticDefaults() => DisplayName.SetDefault("Beach Umbrella");

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<BeachUmbrella>());
		Item.value = Item.buyPrice(0, 0, 20, 0);
		Item.Size = new Microsoft.Xna.Framework.Vector2(30, 34);
	}
}