using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Glyphs
{
	public class BeeGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Bee;
		public override Color Color => new(255, 184, 31);

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = Item.CommonMaxStack;
		}

		public static void ReleaseBees(Player owner, NPC target, int damage)
		{
			if ((owner.whoAmI != Main.myPlayer) || !target.CanLeech() || (damage <= 0))
				return;

			int count = Main.rand.Next(1, 3);
			for (int i = 0; i < count; i++)
				if (owner.ownedProjectileCounts[owner.beeType()] < 12)
				{
					Vector2 velocity = target.velocity.RotatedByRandom(1f);
					Projectile.NewProjectile(owner.GetSource_OnHit(target), target.Center + velocity, velocity, owner.beeType(), owner.beeDamage(damage), 0, owner.whoAmI);
				}
		}

		public static void HoneyEffect(Player owner)
		{
			owner.AddBuff(BuffID.Honey, 300);
			for (int i = 0; i < 12; i++)
				Dust.NewDustDirect(owner.position, owner.width, owner.height, DustID.Honey, 0, 1, 100, default, Main.rand.NextFloat(1f, 2f)).noGravity = true;
		}
	}
}