using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Glyph;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Glyphs
{
	public class UnholyGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Unholy;
		public override Color Color => new(176, 221, 44);

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
		}

		public static void Erupt(Player owner, NPC target, int damage)
		{
			if (owner.whoAmI != Main.myPlayer)
				return;

			int type = ModContent.ProjectileType<CursedPhantom>();
			int count = Main.rand.Next(2, 5);

			for (int i = 0; i < count; i++)
			{
				if (owner.ownedProjectileCounts[type] >= 10)
					break;
				Projectile.NewProjectileDirect(owner.GetSource_OnHit(target), target.Center, (Vector2.UnitY * -Main.rand.NextFloat(8f, 12f)).RotatedByRandom(1.5f), type, damage, 4, owner.whoAmI, ai1: Main.rand.Next(100, 180)).scale = Main.rand.NextFloat(.8f, 1f);
			}
		}
	}
}