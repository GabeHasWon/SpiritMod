using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Glyph;
using SpiritMod.Particles;
using SpiritMod.Projectiles.Glyph;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Glyphs
{
	public class FrostGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Frost;
		public override Color Color => new(49, 209, 215);

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 999;
		}

		public static void FreezeEffect(Player owner, NPC target, Projectile proj)
		{
			bool projAttack = proj != null;
			Vector2 position = projAttack ? proj.Center : target.Hitbox.ClosestPointInRect(owner.Center);
			Vector2 velocity = (projAttack && proj.velocity != Vector2.Zero) ? -Vector2.Normalize(proj.velocity) : target.DirectionTo(owner.Center);

			for (int i = 0; i < 15; i++)
			{
				Vector2 newVel = velocity * Main.rand.NextFloat(.25f, 2.5f);

				if (i < 5)
					ParticleHandler.SpawnParticle(new SmokeParticle(position, newVel.RotatedByRandom(1f), Color.LightBlue with { A = 0 }, Main.rand.NextFloat(.4f, .8f), Main.rand.Next(10, 20)));

				Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool() ? DustID.AncientLight : DustID.IceGolem, newVel.RotatedByRandom(1.5f), 0, default, Main.rand.NextFloat(.5f, 1.5f));
				dust.fadeIn = 1.2f;
				dust.noGravity = true;
			}
			for (int i = 0; i < 3; i++)
				Projectile.NewProjectile(owner.GetSource_OnHit(target), position, (velocity * Main.rand.NextFloat(4f, 8f)).RotatedByRandom(2f), ModContent.ProjectileType<MagicSpiral>(), 0, 0, owner.whoAmI);

			float value = MathHelper.Max(owner.HeldItem.rare / ItemRarityID.Count, 0); //Increase in power based on the player's held item rarity
			float intensity = .002f - (.0015f * value);
			target.AddBuff(ModContent.BuffType<ArcaneFreeze>(), (int)MathHelper.Clamp((1f - (target.lifeMax * intensity)) * 100, 30, 220));
		}
	}
}