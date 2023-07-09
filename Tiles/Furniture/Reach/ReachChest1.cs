using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.Reach
{
	public class ReachChest1 : BaseChest
	{
		public override string DefaultName => "Elderbark Chest";

		public override void StaticDefaults(ModTranslation name)
		{
			AddMapEntry(new Color(179, 146, 107), name, MapChestName);
			ChestDrop = ModContent.ItemType<Items.Placeable.Furniture.Reach.ReachChest>();
			DustType = DustID.RichMahogany;;
		}
	}
}