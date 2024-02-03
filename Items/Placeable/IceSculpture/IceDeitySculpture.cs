using SpiritMod.Tiles.Ambient.IceSculpture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.IceSculpture
{
	[Sacrifice(1)]
	public class IceDeitySculpture : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 40;
			Item.value = Item.buyPrice(0, 10, 0, 0);
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<IceDeityDecor>();
		}
	}
}