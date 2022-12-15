using Terraria;

namespace SpiritMod.Items
{
	public abstract class MinionAccessory : AccessoryItem
	{
		public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) => !(equippedItem.ModItem is MinionAccessory && incomingItem.ModItem is MinionAccessory);
	}
}
