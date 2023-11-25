using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace SpiritMod.Mechanics.SpecialSellItem
{
	public class SpecialSellItem : GlobalItem
	{
		public static bool IsSellItem(Item item) => item.ModItem is ISpecialSellItem sellItem && sellItem.SellAmount() > 0;

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
				string name = (specialSell.SellName() == string.Empty) ? currencyItem.DisplayName.Value : (item.ModItem as ISpecialSellItem).SellName();

				priceLine.Text = $"{Language.GetTextValue("LegacyTooltip.49")} {customPrice} " + name + ((customPrice > 1) ? "s" : string.Empty);
				priceLine.OverrideColor = specialSell.SellColor();
			}
		}
	}

	public class SpecialSellPlayer : ModPlayer
	{
		public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
		{
			if (SpecialSellItem.IsSellItem(item))
			{
				item.shopCustomPrice = null;
				item.shopSpecialCurrency = -1;
			}
		}

		public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item)
		{
			if (SpecialSellItem.IsSellItem(item))
			{
				var specialSell = item.ModItem as ISpecialSellItem;

				int numLoops = (item.stack / item.maxStack) + 1;
				for (int i = 0; i < numLoops; i++)
					Player.QuickSpawnItem(Player.GetSource_Misc("SellItem"), specialSell.SellType(), item.stack / numLoops * specialSell.SellAmount());
				
				item.shopSpecialCurrency = specialSell.CurrencyID();
				item.shopCustomPrice = specialSell.SellAmount();
			}
			return base.CanSellItem(vendor, shopInventory, item);
		}

		public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
		{
			if (SpecialSellItem.IsSellItem(item))
				Player.BuyItem(item.stack); //Fixes players gaining copper coins based on item stack when they shouldn't
		}
	}

	/// <summary>
	/// Allows an item to be sold and repurchased for the specified special currency.
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