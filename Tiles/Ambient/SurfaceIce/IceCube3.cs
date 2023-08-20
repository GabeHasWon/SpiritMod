using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System.Collections.Generic;

namespace SpiritMod.Tiles.Ambient.SurfaceIce
{
	public class IceCube3 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolidTop[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.addTile(Type);
			DustType = DustID.Ice;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.BreakableWhenPlacing[Type] = true;
		}

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
		
		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) => SoundEngine.PlaySound(SoundID.Item27);

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			if (Main.rand.NextBool(3))
				yield return new Item(ModContent.ItemType<Items.Sets.FrigidSet.FrigidFragment>());
		}
	}
}