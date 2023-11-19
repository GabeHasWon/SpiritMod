using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Glyphs
{
	public class BlazeGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Blaze;
		public override Color Color => new(233, 143, 26);

		public override void SetDefaults()
		{
			Item.height = Item.width = 28;
			Item.rare = ItemRarityID.Pink;
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}