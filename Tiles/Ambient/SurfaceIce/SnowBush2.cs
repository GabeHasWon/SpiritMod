using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using SpiritMod.Items.Consumable.Food;
using System.Collections.Generic;

namespace SpiritMod.Tiles.Ambient.SurfaceIce
{
	public class SnowBush2 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolidTop[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16
			};
			TileObjectData.addTile(Type);
            HitSound = SoundID.Grass;
            DustType = DustID.GrassBlades;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.BreakableWhenPlacing[Type] = true;
		}

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			if (Main.rand.NextBool(5))
				yield return new Item(ModContent.ItemType<IceBerries>());
		}
	}
}