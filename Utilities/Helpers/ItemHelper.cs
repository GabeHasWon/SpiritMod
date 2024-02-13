using Terraria.DataStructures;
using Terraria.ID;
using Terraria;

namespace SpiritMod.Utilities.Helpers;

internal class ItemHelper
{
	/// <summary>
	/// Spawns an item and automatically syncs it in multiplayer. This is only important for multiplayer clients, as servers sync items automatically.<br/>
	/// This may still be useful for not casting x, y.
	/// </summary>
	public static int SpawnItem(IEntitySource source, float x, float y, int width, int height, int type, int stack = 1)
	{
		int item = Item.NewItem(source, (int)x, (int)y, width, height, type, stack);

		if (Main.netMode == NetmodeID.MultiplayerClient)
			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item);

		return item;
	}
}
