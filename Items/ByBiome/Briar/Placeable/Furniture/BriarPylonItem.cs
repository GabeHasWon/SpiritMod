using SpiritMod.Tiles.Furniture.Pylons;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.ByBiome.Briar.Placeable.Furniture
{
	[Sacrifice(1)]
	internal class BriarPylonItem : ModItem
	{
		public override LocalizedText Tooltip => Language.GetText("CommonItemTooltip.TeleportationPylon");

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<BriarPylonTile>());
			Item.SetShopValues(ItemRarityColor.Blue1, Terraria.Item.buyPrice(gold: 10));
		}
	}
}
