using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.ScarabeusDrops
{
	public class Chitin : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 18;
			Item.value = 20;
			Item.rare = ItemRarityID.Blue;
            Item.value = 700;
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}
