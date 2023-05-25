using Microsoft.Xna.Framework;
using MonoMod.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class PlaceEvil : ChanceEffect
	{
		public override byte WhoAmI => 6;

		public override bool Unlucky => true;

		public override bool Selectable(Point16 tileCoords)
			=> Fathomless_Chest.CheckTileRange(tileCoords.X, tileCoords.Y, new int[] { TileID.Stone, TileID.Dirt, TileID.IceBlock }, 22);

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			for (int value = 0; value < 32; value++)
			{
				int num = Dust.NewDust(new Vector2(tileCoords.X * 16, tileCoords.Y * 16), 50, 50, DustID.Wraith, 0f, -2f, 0, default, 2f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].scale *= .35f;
				Main.dust[num].fadeIn += .1f;
			}

			Dictionary<int, int> pair = new Dictionary<int, int>();

			if (WorldGen.crimson)
				pair.AddRange(new Dictionary<int, int>
			{
				{ TileID.Stone, TileID.Ebonstone },
				{ TileID.Dirt, 23 },
				{ TileID.IceBlock, TileID.CorruptIce}
			});
			else
				pair.AddRange(new Dictionary<int, int>
			{
				{ TileID.Stone, TileID.Crimstone },
				{ TileID.Dirt, 199 },
				{ TileID.IceBlock, TileID.FleshIce}
			});
			Fathomless_Chest.ConvertTiles(tileCoords.X, tileCoords.Y, 22, pair);
		}
	}
}