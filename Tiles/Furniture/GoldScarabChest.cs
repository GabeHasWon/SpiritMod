using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace SpiritMod.Tiles.Furniture
{
	public class GoldScarabChest : BaseChest
	{
		public override int ChestDrop => ItemID.GoldChest;

		public override void StaticDefaults(LocalizedText name)
		{
			Main.tileShine[Type] = 1200;
			AddMapEntry(new Color(232, 193, 0), name, MapChestName);
			DustType = DustID.Gold;
		}
	}
}