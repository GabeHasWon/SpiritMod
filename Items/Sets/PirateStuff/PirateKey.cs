using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace SpiritMod.Items.Sets.PirateStuff
{
	public class PirateKey : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(0, 1, 0, 0);
		}
	}
}
