using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Tiles.BaseTile;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture
{
	public class ReachChest : BaseChest
	{
		public override int ChestDrop => ModContent.ItemType<ReachChestTile>();

		public override void StaticDefaults(LocalizedText name)
		{
			AddMapEntry(new Color(60, 150, 40), name);
			DustType = DustID.RichMahogany;
		}
	}
}