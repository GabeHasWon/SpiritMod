using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.SpaceJunk
{
	public class AsteroidChest : BaseChest
	{
		public override string DefaultName => "Asteroid Chest";

		public override void StaticDefaults(ModTranslation name)
		{
			Main.tileShine[Type] = 1200;
			AddMapEntry(new Color(125, 116, 115), name);
			ChestDrop = ModContent.ItemType<Items.Placeable.Furniture.AsteroidChest>();
			DustType = DustID.Dirt;
		}
    }
}