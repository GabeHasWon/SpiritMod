using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Material
{
	public class SpiritKoi : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 42;
			Item.value = 100;
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}
