using SpiritMod.NPCs.Critters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	public class CleftItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cleft Hopper");
			// Tooltip.SetDefault("'Its shell is quite sturdy'");
		}

		public override void SetDefaults()
		{
			Item.width = Item.height = 20;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = 99;
			Item.noUseGraphic = true;
			Item.value = Item.sellPrice(0, 0, 2, 0);
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 20;
			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = true;
			Item.makeNPC = ModContent.NPCType<Cleft>();
		}
	}
}
