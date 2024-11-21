using SpiritMod.NPCs.Critters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	public class GildJellyItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 18;
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(0, 3, 0, 0);
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 20;
            Item.bait = 55;
			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = true;
			Item.makeNPC = ModContent.NPCType<GildJelly>();
		}
	}
}
