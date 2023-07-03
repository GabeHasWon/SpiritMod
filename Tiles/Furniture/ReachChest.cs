using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Tiles.BaseTile;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture
{
	public class ReachChest : BaseChest
	{
		public override string DefaultName => "Briar Chest";

		public override void StaticDefaults(ModTranslation name)
		{
			AddMapEntry(new Color(60, 150, 40), name);
			ChestDrop = ModContent.ItemType<ReachChestTile>();
			DustType = DustID.RichMahogany;
		}
	}
}