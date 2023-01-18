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
			VanillaMergeAll(TileID.Stone);
		}

		private static void VanillaMergeAll(int tileID)
		{
			for (int i = 0; i < Main.tileMerge.Length; ++i)
			{
				if (!Main.tileFrameImportant[i])
					Main.tileMerge[i][tileID] = true;
			}

			for (int i = 0; i < Main.tileMerge[tileID].Length; ++i)
			{
				if (!Main.tileFrameImportant[i])
					Main.tileMerge[tileID][i] = true;
			}
		}

		public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
			if (Lighting.Brightness(i, j) <= 0 || Main.tileFrameImportant[type])
				return;

			Dictionary<int, List<Offset>> containers = new();

			void CheckLoc(Offset offset, int x, int y)
			{
				Tile tile = Main.tile[x, y];
				int tileType = tile.TileType;

				if (!tile.HasTile || tileType == type)
					return; //Active, duplicate

				if ((Mergers.TypeHas(type, tileType) && Mergers.TypeHas(tileType, type) && type > tileType) || (Mergers.HasUniversal(type) && Mergers.HasUniversal(tileType) && type > tileType))
					return; //Double merge checks

				if (tileType == TileID.Dirt && (Main.tileMergeDirt[type] || TileID.Sets.Grass[type]))
					return; //Dirt and grass tilesheet check

				if (type == TileID.Dirt && (Main.tileMergeDirt[tileType] || TileID.Sets.Grass[tileType]))
					return; //Dirt and grass tilesheet check (reversed)

				if ((tileType == TileID.Mud && type == TileID.Dirt) || (tileType == TileID.Dirt && type == TileID.Mud))
					return; //Hardcoded mud check for some reason

				if (Mergers.TypeHas(type, tileType) || Mergers.HasUniversal(tileType))
				{
					if (containers.ContainsKey(tileType))
						containers[tileType].Add(offset);
					else
						containers.Add(tileType, new List<Offset>() { offset });
				}
			}

			CheckLoc(Offset.Top, i, j - 1);
			CheckLoc(Offset.Bottom, i, j + 1);
			CheckLoc(Offset.Right, i + 1, j);
			CheckLoc(Offset.Left, i - 1, j);

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

			int frameX = 0;
			int frameY = 0;
			int randomOffset = (i + j) % 3;

			FrameOverlay(offsets, ref frameX, ref frameY, randomOffset);
			ActuallyDrawOverlay(type, i, j, frameX, frameY);
		}

		private static void FrameOverlay(List<Offset> offsets, ref int frameX, ref int frameY, int frameOffset)
		{
			if (offsets.Count == 1)
			{
				Offset offset = offsets.First();

				if (offset == Offset.Top)
				{
					frameX = 108 + (frameOffset * 18);
					frameY = 54;
				}
				else if (offset == Offset.Bottom)
				{
					frameX = 108 + (frameOffset * 18);
					frameY = 0;
				}
				else if (offset == Offset.Right)
				{
					frameX = 162;
					frameY = frameOffset * 18;
				}
				else if (offset == Offset.Left)
				{
					frameX = 216;
					frameY = frameOffset * 18;
				}
			}
			else if (offsets.Count == 2)
			{
				if (offsets.Contains(Offset.Left) && offsets.Contains(Offset.Right)) //Straight right/left
				{
					frameX = 108 + (frameOffset * 18);
					frameY = 72;
				}
				else if (offsets.Contains(Offset.Top) && offsets.Contains(Offset.Bottom)) //Straight up/down
				{
					frameX = 90;
					frameY = frameOffset * 18;
				}
				else if (offsets.Contains(Offset.Bottom) && offsets.Contains(Offset.Right)) //Corner right/bottom
				{
					frameX = frameOffset * 36;
					frameY = 54;
				}
				else if (offsets.Contains(Offset.Bottom) && offsets.Contains(Offset.Left)) //Corner left/bottom
				{
					frameX = 18 + (frameOffset * 36);
					frameY = 54;
				}
				else if (offsets.Contains(Offset.Top) && offsets.Contains(Offset.Right)) //Corner top/right
				{
					frameX = frameOffset * 36;
					frameY = 72;
				}
				else if (offsets.Contains(Offset.Top) && offsets.Contains(Offset.Left)) //Corner top/left
				{
					frameX = 18 + (frameOffset * 36);
					frameY = 72;
				}
			}
			else if (offsets.Count == 3)
			{
				if (offsets.Contains(Offset.Top, Offset.Right, Offset.Bottom)) //Merge all but left
				{
					frameX = 0;
					frameY = frameOffset * 18;
				}
				else if (offsets.Contains(Offset.Top, Offset.Left, Offset.Bottom)) //Merge all but right
				{
					frameX = 72;
					frameY = frameOffset * 18;
				}
				else if (offsets.Contains(Offset.Right, Offset.Left, Offset.Bottom)) //Merge all but top
				{
					frameX = 18 + (frameOffset * 18);
					frameY = 0;
				}
				else if (offsets.Contains(Offset.Right, Offset.Left, Offset.Top)) //Merge all but bottom
				{
					frameX = 18 + (frameOffset * 18);
					frameY = 36;
				}
			}
			else if (offsets.Count == 4) //All four
			{
				frameX = 18 + (frameOffset * 18);
				frameY = 18;
			}
		}

		private void ActuallyDrawOverlay(int type, int i, int j, int frameX, int frameY)
		{
			Asset<Texture2D> tex = GetMergeTexture(type);
			Color light = Lighting.GetColor(i, j);
			Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + new Vector2(Main.offScreenRange);

			Main.spriteBatch.Draw(tex.Value, pos, new Rectangle(frameX, frameY, 16, 16), light);
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
				if (x == 1)
					return Offset.Left;
				else if (x == -1)
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

