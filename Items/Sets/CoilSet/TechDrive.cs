using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CoilSet
{
	public class TechDrive : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 24;
			Item.value = 100;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}
