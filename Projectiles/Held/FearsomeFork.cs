using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Held
{
	public class FearsomeFork : ModProjectile
	{
		private const int ExtensionSize = 20;

		private float MeleeSpeed => (Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee) - 1) * 30 + 1;

		int timer = 0;
		
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Trident);
			AIType = ProjectileID.Trident;
		}

		public override void AI()
		{
			if (++timer % 7 == 1) 
			{
				var pos = Projectile.Center + Projectile.velocity * 4;
				var vel = new Vector2(Main.rand.NextFloat(0, 0.5f), 0).RotatedByRandom(MathHelper.TwoPi) + (Projectile.velocity * 0.05f);
				int newProj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<Pumpkin>(), Projectile.damage, 0, Projectile.owner);
				Main.projectile[newProj].DamageType = DamageClass.Melee;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			SpriteEffects effects = (Projectile.direction < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			float rotation = Projectile.rotation + (Projectile.direction >= 0 ? MathHelper.PiOver4 : -MathHelper.PiOver4);
			Texture2D wave = TextureAssets.Extra[98].Value;
			float scale = Projectile.Distance(Main.player[Projectile.owner].Center) / 120f;
			Vector2 origin = new Vector2(wave.Width / 2f, wave.Height * 0.75f);
			var wavePos = Projectile.Center - Main.screenPosition - Projectile.velocity * 4;
			Color baseColor = new(249, 81, 0);
			scale *= (MeleeSpeed - 1) * 0.3f + 1; // Scale to melee speed

			Main.EntitySpriteDraw(wave, wavePos, null, baseColor * 0.45f, rotation, origin, Projectile.scale * scale, effects, 0);
			Main.EntitySpriteDraw(wave, wavePos, null, baseColor * 0.25f, rotation, origin, new Vector2(Projectile.scale * 0.9f, Projectile.scale * 1.25f) * scale, effects, 0);
			Main.EntitySpriteDraw(wave, wavePos, null, baseColor * 0.15f, rotation, origin, new Vector2(Projectile.scale * 0.8f, Projectile.scale * 1.5f) * scale, effects, 0);
			return true;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			int halfSize = ExtensionSize / 2;
			Point hitbox = (Projectile.Center - new Vector2(halfSize) + (Projectile.velocity * MeleeSpeed * 2)).ToPoint();

			if (targetHitbox.Intersects(new Rectangle(hitbox.X, hitbox.Y, ExtensionSize, ExtensionSize)))
				return true;

			return base.Colliding(projHitbox, targetHitbox);
		}
	}
}
