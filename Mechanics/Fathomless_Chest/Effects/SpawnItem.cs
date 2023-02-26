using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnItem : ChanceEffect
	{
		public override bool Unlucky => false;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			int item = Item.NewItem(source, (int)(tileCoords.X * 16) + 8, (int)(tileCoords.Y * 16) + 12, 16, 18, 393, 1);
			
			if (Main.netMode != NetmodeID.SinglePlayer && item >= 0)
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);

			int item1 = Item.NewItem(source, (int)(tileCoords.X * 16) + 8, (int)(tileCoords.Y * 16) + 12, 16, 18, 18, 1);
			
			if (Main.netMode != NetmodeID.SinglePlayer && item1 >= 0)
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item1, 1f);
		}
	}
}