using SpiritMod.NPCs.Critters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	[Sacrifice(3)]
	public class GoldLumothItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = Item.height = 32;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 10, 0, 0);
			Item.useTime = Item.useAnimation = 20;
			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = true;
			Item.makeNPC = ModContent.NPCType<GoldLumoth>();
		}
	}
}
