using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Glyphs
{
	[Sacrifice(5)]
	public class Glyph : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 28;
			Item.value = 0;
			Item.rare = -11;
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}