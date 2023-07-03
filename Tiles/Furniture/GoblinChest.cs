using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace SpiritMod.Tiles.Furniture
{
	public class GoblinChest : BaseChest
	{
		public override int ChestDrop => ItemID.DynastyChest;

		public override void StaticDefaults(LocalizedText name)
		{
			Main.tileShine[Type] = 1200;
			AddMapEntry(new Color(120, 82, 49), name, MapChestName);
			DustType = DustID.WoodFurniture;
		}
	}
}