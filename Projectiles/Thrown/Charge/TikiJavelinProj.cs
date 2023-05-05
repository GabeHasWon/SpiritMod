using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Thrown.Charge
{
	public class TikiJavelinProj : JavelinProj
	{
		internal override int ChargeTime => 100;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tiki Javelin");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void AI()
		{
			if (Released && Main.rand.NextBool(8))
				Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, default, 1.85f).noGravity = true;
		}

		public override void Kill(int timeLeft)
		{
			if (!Released)
				return;

			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

			for (int i = 0; i < 20; i++)
				Dust.NewDustPerfect(Projectile.Center, DustID.Torch, -(Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(1.0f, 3.0f)).RotatedByRandom(1f), 0, default, 1.85f).noGravity = true;
		}

		public override void PostDraw(Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

			SpriteEffects effects = (player.direction < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Vector2 origin = (effects == SpriteEffects.None) ? new Vector2(texture.Width - (Projectile.width / 2), Projectile.height / 2) : Projectile.Size / 2;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, effects, 0);
		}
	}
}
