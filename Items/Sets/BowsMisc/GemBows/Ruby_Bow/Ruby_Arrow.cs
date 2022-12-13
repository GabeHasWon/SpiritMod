using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace SpiritMod.Items.Sets.BowsMisc.GemBows.Ruby_Bow
{
	public class Ruby_Arrow : GemArrow
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ruby Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; 
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		protected override void SafeSetDefaults()
		{
			dustType = DustID.GemRuby;
			glowColor = Color.Red;
		}

		private int bounces = 2;
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			bounces--;
			if (bounces <= 0)
				Projectile.Kill();
			else
			{
				SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.4f });
				for (int index = 0; index < 5; ++index)
				{
					int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
					Main.dust[i].noGravity = true;
				}				
				if (Projectile.velocity.X != oldVelocity.X)
					Projectile.velocity.X = -oldVelocity.X;

				if (Projectile.velocity.Y != oldVelocity.Y)
					Projectile.velocity.Y = -oldVelocity.Y;

				Projectile.velocity *= 0.825f;
			}
			return false;
		}
	}
}
