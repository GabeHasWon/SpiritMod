using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SpiritBiomeDrops
{
	public class NetherCrystal : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = Item.height = 22;
			Item.rare = ItemRarityID.Pink;
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}
