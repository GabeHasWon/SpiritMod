using SpiritMod.GlobalClasses.Players;
using SpiritMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace SpiritMod.Items.Fishing;

public class FisheyeGem : AccessoryItem
{
	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 18;
		Item.value = Item.buyPrice(0, 5, 0, 0);
		Item.rare = ItemRarityID.Green;
		Item.accessory = true;
	}

	public override void UpdateInventory(Player player) => player.GetModPlayer<MiscAccessoryPlayer>().accessory[AccName] = true;
}

public class FisheyeGlobalItem : GlobalItem
{
	public override bool InstancePerEntity => true;
	protected override bool CloneNewInstances => true;

	private bool _increasedValue = false;

	public override void UpdateInventory(Item item, Player player)
	{
		if (!ItemSets.Fish.Contains(item.type))
			return;

		bool hasAccessory = player.HasAccessory<FisheyeGem>() || player.HasItem(ModContent.ItemType<FisheyeGem>(), player.bank4.item);

		if (hasAccessory && !_increasedValue)
		{
			item.value = (int)(item.value * 1.5f);
			_increasedValue = true;
		}

		if (!hasAccessory && _increasedValue)
		{
			item.value = (int)(item.value / 1.5f);
			_increasedValue = false;
		}
	}
}