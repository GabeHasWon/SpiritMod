using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Glyphs
{
	public class BeeGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Bee;
		public override Color Color => new(255, 184, 31);
		public override string Effect => "Honeyed";
		public override string Addendum =>
			"Grants honeyed when an enemy is slain\n" +
			"Attacks periodically release bees";

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = 999;
		}

		public static void ReleaseBees(Player owner, Entity target, int damage)
		{
			if ((owner.whoAmI != Main.myPlayer) || (target is NPC npc && !npc.CanLeech()) || (damage <= 0))
				return;

			int count = Main.rand.Next(1, 3);
			for (int i = 0; i < count; i++)
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