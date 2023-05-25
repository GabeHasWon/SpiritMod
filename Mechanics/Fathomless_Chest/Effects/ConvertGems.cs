using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class ConvertGems : ChanceEffect
	{
		public override byte WhoAmI => 3;

		public override bool Unlucky => false;

		public override bool Selectable(Point16 tileCoords)
			=> Fathomless_Chest.CheckTileRange(tileCoords.X, tileCoords.Y, new int[] { TileID.Stone }, 22);

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			int gemType = Main.rand.Next(new int[] { 63, 64, 65, 66, 67, 68 });
			if (Main.netMode != NetmodeID.SinglePlayer)
				gemType = 64;

			Dictionary<int, int> pair = new Dictionary<int, int>
			{
				{ TileID.Stone, gemType }
			};
			Fathomless_Chest.ConvertTiles(tileCoords.X, tileCoords.Y, 22, pair);

			for (int val = 0; val < 22; val++)
			{
				int num = Dust.NewDust(new Vector2(tileCoords.X * 16, tileCoords.Y * 16), 80, 80, DustID.Electric, 0f, -2f, 0, default, 2f);
				Main.dust[num].noGravity = true;
				Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(77, player);
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].scale *= .25f;
			}
		}
	}
}