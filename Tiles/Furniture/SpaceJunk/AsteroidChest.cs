using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.SpaceJunk
{
	public class AsteroidChest : BaseChest
	{
		public override int ChestDrop => ModContent.ItemType<Items.Placeable.Furniture.AsteroidChest>();

		public override void StaticDefaults(LocalizedText name)
		{
			Main.tileShine[Type] = 1200;
			AddMapEntry(new Color(125, 116, 115), name);
			DustType = DustID.Dirt;
		}
    }
}