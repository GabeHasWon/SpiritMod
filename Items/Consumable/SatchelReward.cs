using SpiritMod.Items.Placeable.Furniture.Paintings;
using SpiritMod.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	[Sacrifice(1)]
	public class SatchelReward : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 52;
			Item.height = 32;
			Item.rare = -11;
			Item.maxStack = Item.CommonMaxStack;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.consumable = true;
			Item.value = Item.buyPrice(0, 6, 0, 0);
		}

		public override bool CanRightClick() => true;

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			for (int i = 0; i < 2; ++i)
			{
				itemLoot.AddOneFromOptions(1, ModContent.ItemType<AdvPainting1>(), ModContent.ItemType<AdvPainting1>(), ModContent.ItemType<AdvPainting2>(),
					ModContent.ItemType<AdvPainting3>(), ModContent.ItemType<AdvPainting4>(), ModContent.ItemType<AdvPainting5>(), ModContent.ItemType<AdvPainting6>(),
					ModContent.ItemType<AdvPainting7>(), ModContent.ItemType<AdvPainting8>(), ModContent.ItemType<AdvPainting9>(), ModContent.ItemType<AdvPainting10>(),
					ModContent.ItemType<AdvPainting11>(), ModContent.ItemType<AdvPainting12>(), ModContent.ItemType<AdvPainting13>(), ModContent.ItemType<AdvPainting14>(),
					ModContent.ItemType<AdvPainting15>(), ModContent.ItemType<AdvPainting16>(), ModContent.ItemType<AdvPainting17>(), ModContent.ItemType<AdvPainting18>(),
					ModContent.ItemType<AdvPainting19>(), ModContent.ItemType<AdvPainting20>(), ModContent.ItemType<AdvPainting21>(), ModContent.ItemType<AdvPainting22>(),
					ModContent.ItemType<AdvPainting23>());
			}
		}
	}
}
