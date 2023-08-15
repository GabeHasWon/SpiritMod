using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Glyphs
{
	public class BlazeGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Blaze;
		public override Color Color => new(233, 143, 26);
		public override string Effect => "Blazing";
		public override string Addendum => "Dealing damage sets you ablaze";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blaze Glyph");
			Tooltip.SetDefault($"{Addendum}\n+25% damage and +10% critical strike chance");
		}

		public override void SetDefaults()
		{
			Item.height = Item.width = 28;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.maxStack = 999;
		}
	}
}