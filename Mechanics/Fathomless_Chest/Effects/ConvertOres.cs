using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class ConvertOres : ChanceEffect
	{
		public override byte WhoAmI => 4;

		public override bool Unlucky => false;

		public override bool Selectable(Point16 tileCoords)
			=> Fathomless_Chest.CheckTileRange(tileCoords.X, tileCoords.Y, new int[] { TileID.Stone }, 22);

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			int oreType;
			if (WorldGen.SavedOreTiers.Gold == TileID.Platinum)
				oreType = TileID.Gold;
			else
				oreType = TileID.Platinum;

			Dictionary<int, int> pair = new Dictionary<int, int>
			{
				{ TileID.Stone, oreType }
			};
			Fathomless_Chest.ConvertTiles(tileCoords.X, tileCoords.Y, 22, pair, 0.25f);
		}
	}
}