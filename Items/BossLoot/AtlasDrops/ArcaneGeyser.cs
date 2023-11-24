using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AtlasDrops
{
	public class ArcaneGeyser : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Cyan;
			Item.value = Item.sellPrice(0, 0, 15, 0);
		}
	}
}
