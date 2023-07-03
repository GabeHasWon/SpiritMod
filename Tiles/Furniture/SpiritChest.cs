using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture
{
	public class SpiritChest : BaseChest
	{
		public override int ChestDrop => ModContent.ItemType<Items.Placeable.Furniture.SpiritChest>();

		public override void StaticDefaults(LocalizedText name)
		{
			AddMapEntry(new Color(70, 130, 180), name, MapChestName);
			DustType = DustID.Asphalt;
		}
	}
}