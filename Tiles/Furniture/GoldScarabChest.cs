using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture
{
	public class GoldScarabChest : BaseChest
	{
		public override string DefaultName => "Gold Chest";

		public override void StaticDefaults(ModTranslation name)
		{
			Main.tileShine[Type] = 1200;
			AddMapEntry(new Color(232, 193, 0), name, MapChestName);
			ChestDrop = ItemID.GoldChest;
			DustType = DustID.Gold;
		}
	}
}