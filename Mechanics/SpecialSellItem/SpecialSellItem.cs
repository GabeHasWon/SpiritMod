using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SpiritMod.Mechanics.SpecialSellItem
{
	public class SpecialSellItem : GlobalItem
	{
		private static bool IsSellItem(Item item) => item.ModItem is ISpecialSellItem sellItem && sellItem.SellAmount() > 0;

		public override void Load() => On.Terraria.Player.SellItem += OnSell;

		public override void Unload() => On.Terraria.Player.SellItem -= OnSell;

		public override void UpdateInventory(Item item, Player player)
		{
			if (IsSellItem(item) && (item.shopCustomPrice != null || item.shopSpecialCurrency != -1))
			{
				item.shopCustomPrice = null;
				item.shopSpecialCurrency = -1;
			} //Fixes items displaying their SpecialPrice tooltip as if they're still being sold by an NPC
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (!IsSellItem(item))
				return;

			var priceLine = tooltips.Where(x => x.Name == "Price").FirstOrDefault();
			if (priceLine is TooltipLine priceTip)
			{
				var specialSell = item.ModItem as ISpecialSellItem;

				int customPrice = specialSell.SellAmount();
				ModItem currencyItem = ItemLoader.GetItem(specialSell.SellType());
				string name = (specialSell.SellName() == string.Empty) ? currencyItem.DisplayName.GetDefault() : (item.ModItem as ISpecialSellItem).SellName();

				priceLine.Text = $"Sell price: {customPrice} " + name + ((customPrice > 1) ? "s" : string.Empty);
				priceLine.OverrideColor = specialSell.SellColor();
			}
		}

		public static bool OnSell(On.Terraria.Player.orig_SellItem orig, Player self, Item item, int stack)
		{
			if (IsSellItem(item))
			{
				var specialSell = item.ModItem as ISpecialSellItem;

				self.QuickSpawnItem(self.GetSource_Misc("SellItem"), specialSell.SellType(), item.stack * specialSell.SellAmount());
				item.shopSpecialCurrency = specialSell.CurrencyID();
				item.shopCustomPrice = specialSell.SellAmount();
			}
			return orig(self, item, stack);
		}
	}

	/// <summary>
	/// Allows an item to be sold and repurchased for the specified special currency. 
	/// Remember that an item's default coin value is still considered here.
	/// </summary>
	public interface ISpecialSellItem
	{
		int CurrencyID();

		int SellType();

		int SellAmount() => 1;

		string SellName() => string.Empty;

		Color SellColor() => Color.Orange;
	}
}