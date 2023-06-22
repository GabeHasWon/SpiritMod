using Terraria.ID;
using Terraria.ModLoader;
using BriarUndergroundBoxTile = SpiritMod.Tiles.MusicBox.BriarUndergroundBox;

namespace SpiritMod.Items.Placeable.MusicBox
{
	[Sacrifice(1)]
	public class BriarUndergroundBox : ModItem
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Music Box (The Briar- Underground)");

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<BriarUndergroundBoxTile>();
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.LightRed;
			Item.value = 100000;
			Item.accessory = true;
			Item.canBePlacedInVanityRegardlessOfConditions = true;
		}
	}
}
