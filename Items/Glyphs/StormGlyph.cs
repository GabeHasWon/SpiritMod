using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Glyphs
{
	public class StormGlyph : GlyphBase
	{
		public const float VelocityBoost = 1.5f;

		public override GlyphType Glyph => GlyphType.Storm;
		public override Color Color => new(142, 186, 231);

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 999;
		}
	}
}