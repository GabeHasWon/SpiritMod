using Terraria;
using Terraria.Chat;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using SpiritMod.Tiles.Furniture.Volleyball;

namespace SpiritMod.Mechanics.SportSystem.Volleyball;

internal class VolleyballValidator : Validator
{
	public override bool Validate(int x, int y, out int leftEdge, out int rightEdge, out int netCenter, out int top, out int bottom)
	{
		Tile tile = Main.tile[x, y];

		leftEdge = rightEdge = netCenter = top = bottom = 0;

		if (tile.TileType != ModContent.TileType<VolleyballNet>())
		{
			ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("You must choose a net!"), Color.Red);
			return false;
		}

		int j = y;
		while (Main.tile[x, j].TileType == ModContent.TileType<VolleyballNet>())
			j--;

		netCenter = j;

		j++;
		int height = 1;
		while (Main.tile[x, j].TileType == ModContent.TileType<VolleyballNet>())
		{
			j++;
			height++;
		}

		if (height <= 10)
		{
			ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"Net must be higher than 10 blocks! ({height} blocks tall currently)"), Color.Red);
			return false;
		}

		if (Main.tile[x, j].TileType != ModContent.TileType<VolleyballCourt>())
		{
			ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Net must be placed on a court!"), Color.Red);
			return false;
		}

		int left = 0;
		int i = x;
		int openSpaceHeight = height * 2;

		while (Main.tile[i, j].TileType == ModContent.TileType<VolleyballCourt>())
		{
			i++;
			left++;

			for (int cY = j - 1; cY > j - openSpaceHeight; cY--)
			{
				if (WorldGen.SolidOrSlopedTile(i, cY))
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Court must be an open space!"), Color.Red);
					return false;
				}
			}
		}
		left--;

		int right = 1;
		i = x;
		while (Main.tile[i, j].TileType == ModContent.TileType<VolleyballCourt>())
		{
			i--;
			right++;

			for (int cY = j - 1; cY > j - openSpaceHeight; cY--)
			{
				if (WorldGen.SolidOrSlopedTile(i, cY))
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Court must be an open space!"), Color.Red);
					return false;
				}
			}
		}
		right--;

		if (left + right < 60)
		{
			ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Court must be at least 60 tiles wide!"), Color.Red);
			return false;
		}

		ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("This is a valid court!"), Color.Green);
		leftEdge = x - left;
		rightEdge = x + right;
		top = netCenter - openSpaceHeight;
		bottom = netCenter + height;
		return true;
	}
}
