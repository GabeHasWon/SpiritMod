using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using SpiritMod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic
{
	public class TeethBottom : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Fangs");
		}
		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 14;
			Projectile.friendly = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = 2;
			Projectile.timeLeft = 90;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}
		int counter = 0;
		public override void AI()
		{
			counter++;
			if (counter >= 45) {
				Projectile.friendly = true;
			}
			if (counter == 33) {
				for (int i = 0; i < 10; i++) {
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedMoss, Main.rand.Next(-2, 3), -2);
				}
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<NightmareDust>());
			}
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<SurgingAnguish>(), 180);
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