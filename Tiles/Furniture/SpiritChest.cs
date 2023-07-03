using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture
{
	public class SpiritChest : BaseChest
	{
		public override string DefaultName => "Duskwood Chest";

		public override void StaticDefaults(ModTranslation name)
		{
			AddMapEntry(new Color(70, 130, 180), name, MapChestName);
			ChestDrop = ModContent.ItemType<Items.Placeable.Furniture.SpiritChest>();
			DustType = DustID.Asphalt;
		}
	}
}