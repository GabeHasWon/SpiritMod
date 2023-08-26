using SpiritMod.Tiles.Ambient;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture
{
	[Sacrifice(1)]
	public class BlueprintTwins : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 94;
			Item.height = 62;
			Item.value = 15000;
			Item.rare = ItemRarityID.LightPurple;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.createTile = ModContent.TileType<TwinsPrint>();
		}
	}
}