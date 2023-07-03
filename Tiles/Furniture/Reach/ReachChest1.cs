using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.Reach
{
	public class ReachChest1 : BaseChest
	{
		public override int ChestDrop => ModContent.ItemType<Items.Placeable.Furniture.Reach.ReachChest>();

		public override void StaticDefaults(LocalizedText name)
		{
			AddMapEntry(new Color(179, 146, 107), name, MapChestName);
			DustType = DustID.RichMahogany;;
		}
	}
}