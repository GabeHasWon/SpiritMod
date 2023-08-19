using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Glyphs
{
	public class VeilGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Veil;
		public override Color Color => new(53, 243, 82);
		public override string Effect => "Guarding";
		public override string Addendum => "Dealing damage builds a defensive veil around the user";

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = 999;
		}
	}
}