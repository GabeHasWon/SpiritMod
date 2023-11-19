using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Glyph;
using SpiritMod.Particles;
using SpiritMod.Projectiles.Glyph;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Glyphs
{
	public class RadiantGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Radiant;
		public override Color Color => new(234, 167, 51);

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = Item.CommonMaxStack;
		}

		public static void RadiantStrike(Player owner, NPC target)
		{
			owner.ClearBuff(ModContent.BuffType<DivineStrike>());

			SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot with { Volume = .4f, Pitch = .8f }, target.Center);
			Projectile.NewProjectile(owner.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<RadiantEnergy>(), 0, 0, owner.whoAmI, target.whoAmI);
			for (int i = 0; i < 5; i++)
				ParticleHandler.SpawnParticle(new StarParticle(target.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 2f, Color.Yellow, Main.rand.NextFloat(.1f, .25f), Main.rand.Next(15, 30), .1f));
		}
	}
}