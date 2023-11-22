using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.TideDrops
{
	public class TribalScale : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 42;
			Item.value = 2000;
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}
