using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class BoneFeatherFriendly : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bone Feather");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 4;
			Projectile.timeLeft = 1000;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.Bullet;
		}
		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			for (int i = 0; i < 10; i++) {
				int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Dirt, (float)(Main.rand.Next(8) - 4), (float)(Main.rand.Next(8) - 4), 133);
				Main.dust[d].scale *= .75f;
			}
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++) {
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
			return false;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(160, 160, 160, 100);
		}
	}
}