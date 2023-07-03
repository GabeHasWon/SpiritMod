using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture
{
	public class GoblinChest : BaseChest
	{
		public override string DefaultName => "Dynasty Chest";

		public override void StaticDefaults(ModTranslation name)
		{
			Main.tileShine[Type] = 1200;
			AddMapEntry(new Color(120, 82, 49), name, MapChestName);
			ChestDrop = ItemID.DynastyChest;
			DustType = DustID.WoodFurniture;
		}
	}
}