using Microsoft.Xna.Framework;
using SpiritMod.GlobalClasses.Tiles;
using SpiritMod.Tiles;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Items;

internal class StaffOfRegrowthHarvestingItem : GlobalItem
{
	public override bool? UseItem(Item item, Player player)
	{
		if (item.type == ItemID.StaffofRegrowth || item.type == ItemID.AcornAxe)
		{
			Point target = new Point(Player.tileTargetX, Player.tileTargetY);
			Tile tile = Main.tile[target.X, target.Y];

			if (tile.HasTile && player.InInteractionRange(target.X, target.Y, TileReachCheckSettings.Simple))
			{
				var type = TagGlobalTile.HarvestableHerbs.FirstOrDefault(x => x == tile.TileType);

				if (type != default)
				{
					IHarvestableHerb herb = ModContent.GetModTile(type) as IHarvestableHerb;

					if (herb.CanBeHarvested(target.X, target.Y))
					{
						WorldGen.KillTile(target.X, target.Y);
						return true;
					}
				}
			}
		}
		return null;
	}
}
