using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SpiritMod.Mechanics.SpecialSellItem
{
	public class SpecialSellItem : GlobalItem
	{
		public override void Load() => On.Terraria.Player.SellItem += OnSell;

		public override void Unload() => On.Terraria.Player.SellItem -= OnSell;

		public override void UpdateInventory(Item item, Player player)
		{
			if (item.ModItem is ISpecialSellItem && (item.shopCustomPrice != null || item.shopSpecialCurrency != -1))
			{
				item.shopCustomPrice = null;
				item.shopSpecialCurrency = -1;
			} //Fixes items displaying their SpecialPrice tooltip as if they're still being sold by an NPC
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.ModItem is not ISpecialSellItem)
				return;

			var priceLine = tooltips.Where(x => x.Name == "Price").FirstOrDefault();
			if (priceLine is TooltipLine priceTip)
			{
				int customPrice = item.shopCustomPrice ?? 1;
				ModItem currencyItem = ItemLoader.GetItem((item.ModItem as ISpecialSellItem).SellType());
				string name = ((item.ModItem as ISpecialSellItem).SellName() == string.Empty) ? currencyItem.DisplayName.GetDefault() : (item.ModItem as ISpecialSellItem).SellName();

				priceLine.Text = $"Sell price: {customPrice} " + name + ((customPrice > 1) ? "s" : string.Empty);
				priceLine.OverrideColor = (item.ModItem as ISpecialSellItem).SellColor();
			}
		}

		public static bool OnSell(On.Terraria.Player.orig_SellItem orig, Player self, Item item, int stack)
		{
			if (item.ModItem is ISpecialSellItem specialSell)
			{
				self.QuickSpawnItem(self.GetSource_Misc("SellItem"), specialSell.SellType(), item.stack * specialSell.SellAmount());
				item.shopSpecialCurrency = specialSell.CurrencyID();
				item.shopCustomPrice = 1;
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