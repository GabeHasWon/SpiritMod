using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnItem : ChanceEffect
	{
		public override byte WhoAmI => 9;

		public override bool Unlucky => false;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;

			Item.NewItem(source, (tileCoords.X * 16) + 8, (tileCoords.Y * 16) + 12, 16, 18, ItemID.Compass);
			Item.NewItem(source, (tileCoords.X * 16) + 8, (tileCoords.Y * 16) + 12, 16, 18, ItemID.DepthMeter);
		}
	}
}