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
		public override string Effect => "Temporal";
		public override string Addendum =>
			"Weapon damage increases the faster you move, but decreases when moving slowly\n" +
			"Consecutive hits grant a short burst of speed";

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.rare = ItemRarityID.LightPurple;
			Item.maxStack = 999;
		}

		public override bool CanApply(Item item) => item.IsWeapon() || item.useStyle > ItemUseStyleID.None && item.mountType < 0 && item.shoot <= ProjectileID.None;

		public int SellAmount() => 2;
	}
}