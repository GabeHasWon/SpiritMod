using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Thrown
{
	public class TargetCan : ModProjectile
	{
		private bool shot = false;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Target Can");

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Shuriken);
			Projectile.width = 25;
			Projectile.damage = 0;
			Projectile.height = 25;
			Projectile.DamageType = DamageClass.Ranged;
		}

		public override void AI()
		{
			var list = Main.projectile.Where(x => x.Hitbox.Intersects(Projectile.Hitbox));
			foreach (var proj in list) {
				if (proj.IsRanged() && proj.active && !shot && proj.friendly && !proj.hostile && (proj.width <= 6 || proj.height <= 6)) {
					ImpactFX();
					shot = true;
					Projectile.damage = 110;
					Projectile.velocity = proj.velocity * 2;
					proj.active = false;
					CombatText.NewText(new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height), new Color(255, 155, 0, 100), "Bullseye!");
					Projectile.DamageType = DamageClass.Ranged;
					Projectile.penetrate = 2;
				}
			}
		}

		public override void Kill(int timeLeft) => ImpactFX();

		private void ImpactFX()
		{
			for (int i = 0; i < 15; i++)
				Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Tin, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 0.7f);
			
			SoundEngine.PlaySound(SoundID.NPCHit4 with { PitchVariance = 0.5f }, Projectile.Center);
		}

		//public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		//{
		//    Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
		//    for (int k = 0; k < projectile.oldPos.Length; k++)
		//    {
		//        Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
		//        Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
		//        spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
		//    }
		//    return true;
		//}

	}
}