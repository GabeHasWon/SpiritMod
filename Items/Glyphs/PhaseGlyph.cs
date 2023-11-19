using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.SpecialSellItem;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Glyphs
{
	public class PhaseGlyph : GlyphBase, ISpecialSellItem
	{
		public override GlyphType Glyph => GlyphType.Phase;
		public override Color Color => new(159, 122, 255);

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.rare = ItemRarityID.LightPurple;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override bool CanApply(Item item) => item.IsWeapon() || item.useStyle > ItemUseStyleID.None && item.mountType < 0 && item.shoot <= ProjectileID.None;

		public int SellAmount() => 2;
	}
}