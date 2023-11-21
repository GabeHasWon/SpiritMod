using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Sets.FloatingItems
{
	[Sacrifice(1)]
	public class Ambergris : FloatingItem
	{
		public override float SpawnWeight => 0.01f;
		public override float Weight => base.Weight * 0.9f;
		public override float Bouyancy => base.Bouyancy * 1.08f;

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 20, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}