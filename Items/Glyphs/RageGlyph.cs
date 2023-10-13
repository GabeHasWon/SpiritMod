using Microsoft.Xna.Framework;
using SpiritMod.Particles;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Glyphs
{
	public class RageGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Rage;
		public override Color Color => new(243, 102, 33);

		public override void SetDefaults()
		{
			Item.height = Item.width = 28;
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = 999;
		}

		public static void RageEffect(Player owner, NPC target, Projectile proj)
		{
			Vector2 position = (proj != null) ? target.Hitbox.ClosestPointInRect(proj.Center) : target.Hitbox.ClosestPointInRect(owner.Center);

			for (int i = 0; i < 15; i++)
			{
				if (i < 3)
					ParticleHandler.SpawnParticle(new ImpactLine(position, Main.rand.NextVector2Unit(), Color.Orange, new Vector2(.4f, .8f), Main.rand.Next(10, 20)));

				Dust dust = Dust.NewDustPerfect(position, DustID.HeatRay, Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 5, 0, default, Main.rand.NextFloat(.5f, 1.5f));
				dust.fadeIn = 1.2f;
				dust.noGravity = true;
			}
		}
	}
}