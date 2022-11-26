using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Walls.Natural
{
	public class AsteroidWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			AddMapEntry(new Color(92, 76, 64));
			ItemDrop = ModContent.ItemType<Items.Placeable.Walls.AsteroidWall>();
		}
	}
}