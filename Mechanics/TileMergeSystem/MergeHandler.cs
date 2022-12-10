using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace SpiritMod.Mechanics.TileMergeSystem
{
	internal class MergeHandler : GlobalTile
	{
		const string VanillaMergePath = "Mechanics/TileMergeSystem/Textures/";

		public override bool IsLoadingEnabled(Mod mod) => false;

		public override void SetStaticDefaults()
		{
			VanillaMergeAll(TileID.Dirt);
		}

		private void VanillaMergeAll(int tileID)
		{
			for (int i = 0; i < Main.tileMerge.Length; ++i)
				Main.tileMerge[i][tileID] = true;

			for (int i = 0; i < Main.tileMerge[tileID].Length; ++i)
				Main.tileMerge[tileID][i] = true;
		}

		public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
			if (Lighting.Brightness(i, j) <= 0 || Main.tileFrameImportant[type])
				return;

			Dictionary<int, List<Offset>> containers = new();

			for (int x = i - 1; x < i + 2; ++x)
			{
				for (int y = j - 1; y < j + 2; ++y)
				{
					if (x == i && y == j)
						continue;

					Tile tile = Main.tile[x, y];
					int tileType = tile.TileType;

					if (!tile.HasTile || type == tileType)
						continue;

					if (Mergers.TypeHas(type, tileType) || Mergers.HasUniversal(tileType))
					{
						if (containers.ContainsKey(tileType))
							containers[tileType].Add(GetOffset(i - x, j - y));
						else
							containers.Add(tileType, new List<Offset>() { GetOffset(i - x, j - y) });
					}
				}
			}

			if (containers.Count > 0)
			{
				foreach (int item in containers.Keys)
					RenderOverlays(containers[item], item, i, j);
			}
		}

		private void RenderOverlays(List<Offset> offsets, int type, int i, int j)
		{
			if (offsets.Count == 0 || (offsets.Count == 1 && offsets.First() == Offset.None))
				return;

			//if (offsets.Count == 1)
			//{
			//	Offset offset = offsets.First();

			//	if (offset == Offset.Top)
			//	{
			//		frameX = 108;
			//		frameY = 54;
			//	}
			//	else if (offset == Offset.Bottom)
			//	{
			//		frameX = 108;
			//		frameY = 0;
			//	}
			//	else if (offset == Offset.Right)
			//	{
			//		frameX = 156;
			//		frameY = 0;
			//	}
			//	else if (offset == Offset.Left)
			//	{
			//		frameX = 216;
			//		frameY = 0;
			//	}
			//}

			Tile tile = Main.tile[i, j];
			ActuallyDrawOverlay(type, i, j, tile.TileFrameX, tile.TileFrameY);
		}

		private void ActuallyDrawOverlay(int type, int i, int j, int frameX, int frameY)
		{
			Asset<Texture2D> tex = GetMergeTexture(type);

			Main.spriteBatch.Draw(tex.Value, new Vector2(i, j) * 16 - Main.screenPosition + new Vector2(Main.offScreenRange), new Rectangle(frameX, frameY, 16, 16), Color.White);
		}

		private Asset<Texture2D> GetMergeTexture(int type)
		{
			if (type < TileID.Count)
				return Mod.Assets.Request<Texture2D>(VanillaMergePath + "Merge_" + type);
			return Mod.Assets.Request<Texture2D>(ModContent.GetModTile(type).Texture + "_Merge");
		}

		private static Offset GetOffset(int x, int y)
		{
			if (y == 1)
			{
				if (x == -1)
					return Offset.TopLeft;
				else if (x == 0)
					return Offset.Top;
				else if (x == 1)
					return Offset.TopRight;
			}
			else if (y == 0)
			{
				if (x == -1)
					return Offset.Left;
				else if (x == 1)
					return Offset.Right;
			}
			else if (y == -1)
			{
				if (x == -1)
					return Offset.BottomLeft;
				else if (x == 0)
					return Offset.Bottom;
				else if (x == 1)
					return Offset.BottomRight;
			}
			return Offset.None;
		}

		private enum Offset
		{
			TopLeft,
			Top,
			TopRight,
			Left,
			Right,
			BottomLeft,
			Bottom,
			BottomRight,
			None
		}
	}
}

